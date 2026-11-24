using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.BLL.DTOs.PatientDto;

using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.PL.Helpers;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Patient)]
public class PortalController : Controller
{
    private readonly IPatientService _patients;
    private readonly IAppointmentService _appointments;
    private readonly IClinicClock _clock;

    public PortalController(
        IPatientService patients,
        IAppointmentService appointments,
        IClinicClock clock)
    {
        _patients = patients;
        _appointments = appointments;
        _clock = clock;
    }

    public async Task<IActionResult> Index()
    {
        var patient = await CurrentPatientAsync();
        if (patient is null)
            return View("NotLinked");

        var all = (await _appointments.GetByPatientAsync(patient.Id)).ToList();

        bool IsUpcoming(AppointmentDto a) =>
            a.Status == AppointmentStatus.Booked &&
            (a.Date > _clock.Today || (a.Date == _clock.Today && a.StartTime > _clock.TimeNow));

        return View(new PortalIndexViewModel
        {
            PatientName = patient.FullName,
            Upcoming = all.Where(IsUpcoming).OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList(),
            Past = all.Where(a => !IsUpcoming(a)).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Book(DateOnly? date)
    {
        var patient = await CurrentPatientAsync();
        if (patient is null)
            return View("NotLinked");

        var selected = date ?? _clock.Today;
        var slots = await _appointments.GetAvailableSlotsAsync(selected);

        return View(new BookAppointmentViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            PatientPhone = patient.Phone,
            Date = selected,
            MinDate = _clock.Today,
            Slots = slots.ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(DateOnly date, TimeOnly startTime)
    {
        var patient = await CurrentPatientAsync();
        if (patient is null)
            return View("NotLinked");

        // الـ PatientId بييجي من الحساب المسجل، مش من الفورم
        var result = await _appointments.BookOnlineAsync(new CreateAppointmentDto
        {
            PatientId = patient.Id,
            Date = date,
            StartTime = startTime
        });

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Book), new { date = date.ToIso() });
        }

        TempData["Success"] = "تم حجز موعدك بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var patient = await CurrentPatientAsync();
        if (patient is null)
            return View("NotLinked");

        var result = await _appointments.CancelOwnAsync(id, patient.Id);

        if (result.IsSuccess)
            TempData["Success"] = "تم إلغاء الحجز";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    private async Task<PatientDto?> CurrentPatientAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null ? null : await _patients.GetByUserIdAsync(userId);
    }
}