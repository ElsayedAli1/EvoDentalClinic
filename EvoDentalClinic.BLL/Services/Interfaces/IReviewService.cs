using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Reviews;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IReviewService
{
    // العرض العام
    Task<ReviewSummaryDto> GetPublishedAsync(int take);

    // المريض
    Task<ReviewDto?> GetByUserAsync(string userId);
    Task<Result> SaveAsync(string userId, SaveReviewDto dto);
    Task<Result> DeleteOwnAsync(string userId);

    // الدكتور
    Task<List<ReviewModerationDto>> GetByStatusAsync(ReviewStatus status);
    Task<int> CountPendingAsync();
    Task<Result> SetStatusAsync(int id, ReviewStatus status);
}