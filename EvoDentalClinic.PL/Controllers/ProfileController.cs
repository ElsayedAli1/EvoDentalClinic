using System.Security.Claims;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IAccountService _accounts;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IAccountService accounts,
        SignInManager<ApplicationUser> signInManager,
        ILogger<ProfileController> logger)
    {
        _accounts = accounts;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _accounts.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        // تغيير الباسورد بيبوّظ الجلسة القديمة، فبنجدّدها عشان المستخدم ما يتطلّعش
        var user = await _signInManager.UserManager.GetUserAsync(User);
        if (user is not null)
            await _signInManager.RefreshSignInAsync(user);

        _logger.LogWarning("Password changed by {User}", User.Identity?.Name);

        TempData["Success"] = "تم تغيير كلمة المرور";
        return RedirectToAction("Index", "Home");
    }
}