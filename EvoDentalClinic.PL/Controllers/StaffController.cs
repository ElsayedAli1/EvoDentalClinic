using EvoDentalClinic.BLL.DTOs.Staff;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor)]
public class StaffController : Controller
{
    private readonly IStaffService _staff;

    public StaffController(IStaffService staff)
    {
        _staff = staff;
    }

    public async Task<IActionResult> Index()
        => View(await _staff.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new CreateStaffViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStaffViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _staff.CreateReceptionistAsync(new CreateStaffDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم إنشاء حساب الاستقبال";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(string id, bool active)
    {
        var result = await _staff.SetActiveAsync(id, active);

        if (result.IsSuccess)
            TempData["Success"] = active ? "تم تفعيل الحساب" : "تم إيقاف الحساب";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var staff = await _staff.GetByIdAsync(id);
        if (staff is null)
            return NotFound();

        return View(new ResetStaffPasswordViewModel { UserId = staff.Id, FullName = staff.FullName });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetStaffPasswordViewModel model)
    {
        var staff = await _staff.GetByIdAsync(model.UserId);
        if (staff is null)
            return NotFound();

        model.FullName = staff.FullName;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _staff.ResetPasswordAsync(model.UserId, model.NewPassword);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم تغيير كلمة المرور";
        return RedirectToAction(nameof(Index));
    }
}