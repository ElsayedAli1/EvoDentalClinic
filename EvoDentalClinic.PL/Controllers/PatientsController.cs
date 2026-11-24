using EvoDentalClinic.BLL.DTOs.PatientDto;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]


public class PatientsController : Controller
{
    private readonly IPatientService _patients;
    private readonly IAppointmentService _appointments;
    private readonly ITreatmentService _treatments;
    private readonly IPatientAccountService _accounts;

    public PatientsController(
        IPatientService patients,
        IAppointmentService appointments,
        ITreatmentService treatments,
        IPatientAccountService accounts)
    {
        _patients = patients;
        _appointments = appointments;
        _treatments = treatments;
        _accounts = accounts;
    }

    // قايمة المرضى + البحث
    public async Task<IActionResult> Index(string? term, int page = 1)
    {
        var result = await _patients.SearchAsync(term, page);

        // لو الصفحة المطلوبة أكبر من آخر صفحة، نرجع لآخر صفحة
        if (result.TotalPages > 0 && page > result.TotalPages)
            return RedirectToAction(nameof(Index), new { term, page = result.TotalPages });

        ViewData["Term"] = term;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new PatientFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new CreatePatientDto
        {
            FullName = model.FullName.Trim(),
            Phone = model.Phone,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender!.Value,
            MaritalStatus = model.MaritalStatus,
            Address = model.Address,
            Job = model.Job
        };

        var result = await _patients.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم تسجيل المريض بنجاح";
        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var patient = await _patients.GetByIdAsync(id);
        if (patient is null)
            return NotFound();

        var model = new PatientFormViewModel
        {
            Id = patient.Id,
            Code = patient.Code,
            FullName = patient.FullName,
            Phone = patient.Phone,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MaritalStatus = patient.MaritalStatus,
            Address = patient.Address,
            Job = patient.Job
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PatientFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new UpdatePatientDto
        {
            Id = model.Id,
            FullName = model.FullName.Trim(),
            Phone = model.Phone,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender!.Value,
            MaritalStatus = model.MaritalStatus,
            Address = model.Address,
            Job = model.Job
        };

        var result = await _patients.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم حفظ التعديلات";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
    public async Task<IActionResult> Details(int id)
    {
        var patient = await _patients.GetByIdAsync(id);
        if (patient is null)
            return NotFound();

        var model = new PatientFileViewModel
        {
            Patient = patient,
            Appointments = (await _appointments.GetByPatientAsync(id)).ToList(),
            Treatments = (await _treatments.GetByPatientAsync(id)).ToList(),
            Account = await _accounts.GetAsync(id)
        };
        return View(model);
    }
}