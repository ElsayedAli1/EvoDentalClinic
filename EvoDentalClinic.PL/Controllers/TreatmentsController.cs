using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Treatments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor)]
public class TreatmentsController : Controller
{
    private readonly ITreatmentService _treatments;
    private readonly IPatientService _patients;
    private readonly IClinicClock _clock;

    public TreatmentsController(
        ITreatmentService treatments,
        IPatientService patients,
        IClinicClock clock)
    {
        _treatments = treatments;
        _patients = patients;
        _clock = clock;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int patientId)
    {
        var patient = await _patients.GetByIdAsync(patientId);
        if (patient is null)
            return NotFound();

        return View(new TreatmentFormViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            Date = _clock.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TreatmentFormViewModel model)
    {
        var patient = await _patients.GetByIdAsync(model.PatientId);
        if (patient is null)
            return NotFound();

        model.PatientName = patient.FullName;

        if (!ModelState.IsValid)
            return View(model);

        var dto = new CreateTreatmentRecordDto
        {
            PatientId = patient.Id,
            Date = model.Date!.Value,
            ToothNumber = model.ToothNumber!.Value,
            Procedure = model.Procedure,
            TotalCost = model.TotalCost!.Value
        };

        var result = await _treatments.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم تسجيل الإجراء";
        return RedirectToAction("Details", "Patients", new { id = patient.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var record = await _treatments.GetByIdAsync(id);
        if (record is null)
            return NotFound();

        var patient = await _patients.GetByIdAsync(record.PatientId);
        if (patient is null)
            return NotFound();

        return View(new TreatmentFormViewModel
        {
            Id = record.Id,
            PatientId = record.PatientId,
            PatientName = patient.FullName,
            Date = record.Date,
            ToothNumber = record.ToothNumber,
            Procedure = record.Procedure,
            TotalCost = record.TotalCost
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TreatmentFormViewModel model)
    {
        // المريض بييجي من الداتا بيز مش من الفورم
        var record = await _treatments.GetByIdAsync(model.Id);
        if (record is null)
            return NotFound();

        var patient = await _patients.GetByIdAsync(record.PatientId);
        if (patient is null)
            return NotFound();

        model.PatientId = patient.Id;
        model.PatientName = patient.FullName;

        if (!ModelState.IsValid)
            return View(model);

        var dto = new UpdateTreatmentRecordDto
        {
            Id = model.Id,
            Date = model.Date!.Value,
            ToothNumber = model.ToothNumber!.Value,
            Procedure = model.Procedure,
            TotalCost = model.TotalCost!.Value
        };

        var result = await _treatments.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم حفظ التعديلات";
        return RedirectToAction("Details", "Patients", new { id = patient.Id });
    }
}