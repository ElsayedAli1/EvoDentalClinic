using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.PL.Helpers;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointments;
    private readonly IPatientService _patients;
    private readonly IClinicClock _clock;

    public AppointmentsController(
        IAppointmentService appointments,
        IPatientService patients,
        IClinicClock clock)
    {
        _appointments = appointments;
        _patients = patients;
        _clock = clock;
    }

    // حجوزات يوم معين (الافتراضي النهاردة)
    public async Task<IActionResult> Index(DateOnly? date)
    {
        var selected = date ?? _clock.Today;

        ViewData["Date"] = selected;
        ViewData["Today"] = _clock.Today;
        ViewData["Now"] = _clock.TimeNow;

        var appointments = await _appointments.GetByDateAsync(selected);
        return View(appointments);
    }

    // شاشة اختيار الميعاد لمريض معين
    [HttpGet]
    public async Task<IActionResult> Book(int patientId, DateOnly? date)
    {
        var patient = await _patients.GetByIdAsync(patientId);
        if (patient is null)
            return NotFound();

        var selected = date ?? _clock.Today;
        var slots = await _appointments.GetAvailableSlotsAsync(selected);

        var model = new BookAppointmentViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            PatientPhone = patient.Phone,
            Date = selected,
            MinDate = _clock.Today,
            Slots = slots.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int patientId, DateOnly date, TimeOnly startTime)
    {
        var dto = new CreateAppointmentDto
        {
            PatientId = patientId,
            Date = date,
            StartTime = startTime
        };

        var result = await _appointments.BookAsync(dto);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Book), new { patientId, date = date.ToIso() });
        }

        TempData["Success"] = "تم الحجز بنجاح";
        return RedirectToAction(nameof(Index), new { date = date.ToIso() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, DateOnly date)
    {
        var result = await _appointments.CancelAsync(id);

        if (result.IsSuccess)
            TempData["Success"] = "تم إلغاء الحجز";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index), new { date = date.ToIso() });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, DateOnly date, AppointmentStatus status)
    {
        var result = await _appointments.SetStatusAsync(id, status);

        if (result.IsSuccess)
            TempData["Success"] = status == AppointmentStatus.Completed
                ? "تم تسجيل الكشف"
                : "تم تسجيل إن المريض لم يحضر";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index), new { date = date.ToIso() });
    }
}