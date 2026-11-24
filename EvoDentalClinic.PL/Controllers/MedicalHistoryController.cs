using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
public class MedicalHistoryController : Controller
{
    private readonly IMedicalHistoryService _histories;
    private readonly IPatientService _patients;
    private readonly ILogger<MedicalHistoryController> _logger;

    public MedicalHistoryController(
        IMedicalHistoryService histories,
        IPatientService patients,
        ILogger<MedicalHistoryController> logger)
    {
        _histories = histories;
        _patients = patients;
        _logger = logger;
    }

    public async Task<IActionResult> Details(int patientId)
    {
        var patient = await _patients.GetByIdAsync(patientId);
        if (patient is null)
            return NotFound();

        return View(new MedicalHistoryDetailsViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            History = await _histories.GetByPatientAsync(patientId)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int patientId)
    {
        var patient = await _patients.GetByIdAsync(patientId);
        if (patient is null)
            return NotFound();

        var history = await _histories.GetByPatientAsync(patientId);

        return View(new MedicalHistoryFormViewModel
        {
            PatientName = patient.FullName,
            IsFemale = patient.Gender == Gender.Female,
            History = history ?? new() { PatientId = patientId }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MedicalHistoryFormViewModel model)
    {
        var patient = await _patients.GetByIdAsync(model.History.PatientId);
        if (patient is null)
            return NotFound();

        model.PatientName = patient.FullName;
        model.IsFemale = patient.Gender == Gender.Female;

        var result = await _histories.SaveAsync(model.History);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        _logger.LogInformation("Medical history saved for patient {PatientId} by {User}",
            patient.Id, User.Identity?.Name);

        TempData["Success"] = "تم حفظ التاريخ المرضي";
        return RedirectToAction(nameof(Details), new { patientId = patient.Id });
    }
}