using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor + "," + Roles.Receptionist)]
public class PatientAccountsController : Controller
{
    private readonly IPatientAccountService _accounts;
    private readonly IPatientService _patients;

    public PatientAccountsController(IPatientAccountService accounts, IPatientService patients)
    {
        _accounts = accounts;
        _patients = patients;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Link(int patientId)
    {
        var result = await _accounts.LinkAsync(patientId);
        Notify(result, "تم ربط الحساب بملف المريض");
        return BackToFile(patientId);
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(int patientId)
    {
        var patient = await _patients.GetByIdAsync(patientId);
        if (patient is null)
            return NotFound();

        var account = await _accounts.GetAsync(patientId);
        if (account.State != AccountState.Linked)
        {
            TempData["Error"] = "تغيير الباسورد للحسابات المربوطة بس";
            return BackToFile(patientId);
        }

        return View(new ResetPatientPasswordViewModel
        {
            PatientId = patientId,
            PatientName = patient.FullName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPatientPasswordViewModel model)
    {
        var patient = await _patients.GetByIdAsync(model.PatientId);
        if (patient is null)
            return NotFound();

        model.PatientName = patient.FullName;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _accounts.ResetPasswordAsync(model.PatientId, model.NewPassword);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["Success"] = "تم تغيير كلمة المرور";
        return BackToFile(model.PatientId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int patientId)
    {
        var result = await _accounts.DeleteAsync(patientId);
        Notify(result, "تم حذف الحساب. بيانات المريض وسجله زي ما هي");
        return BackToFile(patientId);
    }

    private void Notify(Result result, string success)
    {
        if (result.IsSuccess)
            TempData["Success"] = success;
        else
            TempData["Error"] = result.Error;
    }

    private IActionResult BackToFile(int patientId)
        => RedirectToAction("Details", "Patients", new { id = patientId });
}