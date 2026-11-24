using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Reviews;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class ReviewService : IReviewService
{
    private const int MinCommentLength = 10;
    private const int MaxCommentLength = 600;

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _users;

    public ReviewService(IUnitOfWork uow, IMapper mapper, UserManager<ApplicationUser> users)
    {
        _uow = uow;
        _mapper = mapper;
        _users = users;
    }

    // ---------- العرض العام ----------

    public async Task<ReviewSummaryDto> GetPublishedAsync(int take)
    {
        var approved = (await _uow.Reviews.FindAsync(r => r.Status == ReviewStatus.Approved)).ToList();

        return new ReviewSummaryDto
        {
            TotalCount = approved.Count,
            Average = approved.Count == 0 ? 0 : Math.Round(approved.Average(r => r.Rating), 1),
            Items = _mapper.Map<List<ReviewDto>>(
                approved.OrderByDescending(r => r.CreatedAt).Take(take))
        };
    }

    // ---------- المريض ----------

    public async Task<ReviewDto?> GetByUserAsync(string userId)
    {
        var review = await FindByUserAsync(userId);
        return review is null ? null : _mapper.Map<ReviewDto>(review);
    }

    public async Task<Result> SaveAsync(string userId, SaveReviewDto dto)
    {
        if (dto.Rating is < 1 or > 5)
            return Result.Failure("اختار تقييم من 1 لـ 5 نجوم");

        var comment = dto.Comment?.Trim();

        if (string.IsNullOrWhiteSpace(comment) ||
            comment.Length < MinCommentLength || comment.Length > MaxCommentLength)
            return Result.Failure($"الرأي لازم يكون بين {MinCommentLength} و{MaxCommentLength} حرف");

        if (ContainsLink(comment))
            return Result.Failure("مينفعش تحط روابط في الرأي");

        var user = await _users.FindByIdAsync(userId);
        if (user is null || !await _users.IsInRoleAsync(user, Roles.Patient))
            return Result.Failure("كتابة الرأي متاحة لحسابات المرضى بس");

        var existing = await FindByUserAsync(userId);

        if (existing is null)
        {
            await _uow.Reviews.AddAsync(new Review
            {
                UserId = userId,
                DisplayName = FirstName(user.FullName),
                Rating = dto.Rating,
                Comment = comment,
                Status = ReviewStatus.Pending
            });
        }
        else
        {
            // نفس الرأي من غير تغيير: ما نرجّعوش للمراجعة
            if (existing.Rating == dto.Rating && existing.Comment == comment)
                return Result.Success();

            existing.Rating = dto.Rating;
            existing.Comment = comment;
            existing.Status = ReviewStatus.Pending;   // أي تعديل بيرجع للمراجعة
            existing.ReviewedAt = null;
            _uow.Reviews.Update(existing);
        }

        try
        {
            await _uow.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // ضغطتين بسرعة: الـ Unique Index في الداتا بيز رفض التاني
            if (existing is null && await FindByUserAsync(userId) is not null)
                return Result.Failure("رأيك اتسجّل بالفعل");

            throw;
        }

        return Result.Success();
    }

    public async Task<Result> DeleteOwnAsync(string userId)
    {
        var review = await FindByUserAsync(userId);
        if (review is null)
            return Result.Failure("مفيش رأي مسجّل");

        _uow.Reviews.Remove(review);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }

    // ---------- الدكتور ----------

    public async Task<List<ReviewModerationDto>> GetByStatusAsync(ReviewStatus status)
    {
        var reviews = await _uow.Reviews.FindAsync(r => r.Status == status, r => r.User);

        return _mapper.Map<List<ReviewModerationDto>>(
            reviews.OrderByDescending(r => r.CreatedAt));
    }

    public async Task<int> CountPendingAsync()
        => (await _uow.Reviews.FindAsync(r => r.Status == ReviewStatus.Pending)).Count();

    public async Task<Result> SetStatusAsync(int id, ReviewStatus status)
    {
        if (status != ReviewStatus.Approved && status != ReviewStatus.Rejected)
            return Result.Failure("الحالة دي مش مسموحة من هنا");

        var review = await _uow.Reviews.GetByIdAsync(id);
        if (review is null)
            return Result.Failure("الرأي غير موجود");

        review.Status = status;
        review.ReviewedAt = DateTime.UtcNow;

        _uow.Reviews.Update(review);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }

    // ---------- دوال مساعدة ----------

    private async Task<Review?> FindByUserAsync(string userId)
        => (await _uow.Reviews.FindAsync(r => r.UserId == userId)).FirstOrDefault();

    private static string FirstName(string fullName)
    {
        var first = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? "مريض";

        return first.Length > 50 ? first[..50] : first;
    }

    private static bool ContainsLink(string text)
    {
        var t = text.ToLowerInvariant();
        return t.Contains("http") || t.Contains("www.") || t.Contains(".com");
    }
}