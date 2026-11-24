using EvoDentalClinic.BLL.DTOs.Payments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
public class PaymentsController : Controller
{
    private readonly IPaymentService _payments;
    private readonly ITreatmentService _treatments;
    private readonly IPatientService _patients;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService payments,
        ITreatmentService treatments,
        IPatientService patients,
        ILogger<PaymentsController> logger)
    {
        _payments = payments;
        _treatments = treatments;
        _patients = patients;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int treatmentId)
    {
        var page = await BuildPageAsync(treatmentId);
        if (page is null)
            return NotFound();

        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int treatmentId, [Bind(Prefix = "Input")] PaymentInputModel input)
    {
        var page = await BuildPageAsync(treatmentId);
        if (page is null)
            return NotFound();

        page.Input = input;

        if (!ModelState.IsValid)
            return View(page);

        var dto = new CreatePaymentDto
        {
            TreatmentRecordId = treatmentId,
            Amount = input.Amount!.Value,
            Note = string.IsNullOrWhiteSpace(input.Note) ? null : input.Note.Trim()
        };

        var result = await _payments.AddAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(page);
        }
        _logger.LogInformation(
    "Payment {Amount} recorded for treatment {TreatmentId} (patient {PatientId}) by {User}",
    dto.Amount, treatmentId, page.PatientId, User.Identity?.Name);
        TempData["Success"] = "تم تسجيل الدفعة";
        return RedirectToAction("Details", "Patients", new { id = page.PatientId });
    }

    private async Task<PaymentPageViewModel?> BuildPageAsync(int treatmentId)
    {
        var treatment = await _treatments.GetByIdAsync(treatmentId);
        if (treatment is null)
            return null;

        var patient = await _patients.GetByIdAsync(treatment.PatientId);
        if (patient is null)
            return null;

        var history = await _payments.GetByTreatmentAsync(treatmentId);

        return new PaymentPageViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            Treatment = treatment,
            History = history.ToList()
        };
    }
   
}