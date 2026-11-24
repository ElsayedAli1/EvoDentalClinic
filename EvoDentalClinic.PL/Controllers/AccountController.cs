using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;

namespace EvoDentalClinic.PL.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accounts;
    private readonly ILogger<PaymentsController> _logger;
    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        IAccountService accounts,
        ILogger<PaymentsController> logger)
    {
        _signInManager = signInManager;
        _accounts = accounts;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var userName = await _accounts.ResolveUserNameAsync(model.Identifier);

        if (userName is not null)
        {
            var result = await _signInManager.PasswordSignInAsync(
                userName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Account locked out: {Identifier}", model.Identifier);
                ModelState.AddModelError(string.Empty, "الحساب مقفول، جرّب بعد 10 دقايق أو كلّم العيادة");
                return View(model);
            }
        }
        _logger.LogWarning("Failed login attempt for {Identifier} from {Ip}",
    model.Identifier, HttpContext.Connection.RemoteIpAddress);
        // رسالة واحدة لكل الحالات، عشان محدش يعرف مين عنده حساب
        ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _accounts.RegisterAsync(new RegisterPatientDto
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            Password = model.Password,
            Gender = model.Gender!.Value,
            DateOfBirth = model.DateOfBirth
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        var userName = PhoneHelper.Normalize(model.Phone)!;
        await _signInManager.PasswordSignInAsync(
            userName, model.Password, isPersistent: false, lockoutOnFailure: false);

        TempData["Success"] = result.Value == RegistrationOutcome.PendingLink
            ? "تم إنشاء حسابك. رقمك مسجل في العيادة من قبل، فاطلب من موظف الاستقبال يربطه بملفك."
            : "تم إنشاء حسابك بنجاح";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}