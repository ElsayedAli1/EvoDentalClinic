using System.Security.Claims;
using EvoDentalClinic.BLL.DTOs.Reviews;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Patient)]
public class ReviewController : Controller
{
    private readonly IReviewService _reviews;

    public ReviewController(IReviewService reviews)
    {
        _reviews = reviews;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var existing = await _reviews.GetByUserAsync(UserId);

        return View(new ReviewFormViewModel
        {
            Rating = existing?.Rating,
            Comment = existing?.Comment ?? "",
            HasReview = existing is not null,
            CurrentStatus = existing?.Status
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ReviewFormViewModel model)
    {
        var existing = await _reviews.GetByUserAsync(UserId);
        model.HasReview = existing is not null;
        model.CurrentStatus = existing?.Status;

        if (!ModelState.IsValid)
            return View(model);

        var comment = model.Comment.Trim();

        var result = await _reviews.SaveAsync(UserId, new SaveReviewDto
        {
            Rating = model.Rating!.Value,
            Comment = comment
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        var unchanged = existing is not null &&
                        existing.Rating == model.Rating && existing.Comment == comment;

        TempData["Success"] = unchanged
            ? "مفيش تغيير في رأيك"
            : existing is null
                ? "شكراً ليك! رأيك اتبعت وهيظهر بعد ما الدكتور يراجعه"
                : "تم حفظ التعديل، وهيظهر بعد المراجعة";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete()
    {
        var result = await _reviews.DeleteOwnAsync(UserId);

        if (result.IsSuccess)
            TempData["Success"] = "تم مسح رأيك";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }
}