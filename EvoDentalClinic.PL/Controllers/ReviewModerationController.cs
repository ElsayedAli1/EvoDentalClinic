using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor)]
public class ReviewModerationController : Controller
{
    private readonly IReviewService _reviews;
    private readonly ILogger<ReviewModerationController> _logger;

    public ReviewModerationController(IReviewService reviews, ILogger<ReviewModerationController> logger)
    {
        _reviews = reviews;
        _logger = logger;
    }

    public async Task<IActionResult> Index(ReviewStatus status = ReviewStatus.Pending)
    {
        if (!Enum.IsDefined(status))
            status = ReviewStatus.Pending;

        return View(new ReviewModerationViewModel
        {
            Status = status,
            Items = await _reviews.GetByStatusAsync(status),
            PendingCount = await _reviews.CountPendingAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, ReviewStatus status, ReviewStatus returnTo)
    {
        var result = await _reviews.SetStatusAsync(id, status);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Review {ReviewId} set to {Status} by {User}",
                id, status, User.Identity?.Name);

            TempData["Success"] = status == ReviewStatus.Approved ? "تمت الموافقة على الرأي" : "تم رفض الرأي";
        }
        else
        {
            TempData["Error"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { status = returnTo });
    }
}