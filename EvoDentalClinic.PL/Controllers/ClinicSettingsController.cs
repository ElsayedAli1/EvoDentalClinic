using EvoDentalClinic.BLL.DTOs.Clinic;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor)]
public class ClinicSettingsController : Controller
{
    private readonly IClinicSettingsService _settings;

    public ClinicSettingsController(IClinicSettingsService settings)
    {
        _settings = settings;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var current = await _settings.GetAsync();
        if (current is null)
            return NotFound();

        ViewData["SlotsPerDay"] = current.SlotsPerDay;

        return View(new ClinicSettingsFormViewModel
        {
            WorkDays = current.WorkDays,
            OpenTime = current.OpenTime,
            CloseTime = current.CloseTime,
            SlotMinutes = current.SlotMinutes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ClinicSettingsFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _settings.UpdateAsync(new ClinicSettingsDto
        {
            WorkDays = model.WorkDays,
            OpenTime = model.OpenTime!.Value,
            CloseTime = model.CloseTime!.Value,
            SlotMinutes = model.SlotMinutes!.Value
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم حفظ إعدادات العيادة";
        return RedirectToAction(nameof(Index));
    }
}