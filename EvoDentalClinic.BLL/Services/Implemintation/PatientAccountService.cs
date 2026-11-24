using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using Microsoft.AspNetCore.Identity;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class PatientAccountService : IPatientAccountService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly IUnitOfWork _uow;

    public PatientAccountService(UserManager<ApplicationUser> users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<PatientAccountDto> GetAsync(int patientId)
    {
        var (_, user, state) = await ResolveAsync(patientId);

        return new PatientAccountDto
        {
            State = state,
            UserName = user?.UserName,
            Email = user?.Email
        };
    }

    public async Task<Result> LinkAsync(int patientId)
    {
        var (patient, user, state) = await ResolveAsync(patientId);

        if (patient is null)
            return Result.Failure("المريض غير موجود");

        if (state == AccountState.Linked)
            return Result.Failure("حساب المريض ده مربوط بالفعل");

        if (state != AccountState.Pending || user is null)
            return Result.Failure("مفيش حساب أونلاين برقم المريض ده");

        patient.UserId = user.Id;
        _uow.Patients.Update(patient);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(int patientId, string newPassword)
    {
        var (patient, user, state) = await ResolveAsync(patientId);

        if (patient is null)
            return Result.Failure("المريض غير موجود");

        if (state != AccountState.Linked || user is null)
            return Result.Failure("تغيير الباسورد للحسابات المربوطة بس");

        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var result = await _users.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded ? Result.Success() : Result.Failure(result.ToMessage());
    }

    public async Task<Result> DeleteAsync(int patientId)
    {
        var (patient, user, state) = await ResolveAsync(patientId);

        if (patient is null)
            return Result.Failure("المريض غير موجود");

        if (state == AccountState.None || user is null)
            return Result.Failure("مفيش حساب أونلاين للمريض ده");

        // أول حاجة نفك الربط، وبعدين نمسح الحساب. بيانات المريض بتفضل
        if (state == AccountState.Linked)
        {
            patient.UserId = null;
            _uow.Patients.Update(patient);
            await _uow.SaveChangesAsync();
        }

        var deleted = await _users.DeleteAsync(user);
        return deleted.Succeeded ? Result.Success() : Result.Failure(deleted.ToMessage());
    }

    // ---------- دوال مساعدة ----------

    private async Task<(Patient? Patient, ApplicationUser? User, AccountState State)> ResolveAsync(int patientId)
    {
        var patient = await _uow.Patients.GetByIdAsync(patientId);
        if (patient is null)
            return (null, null, AccountState.None);

        // مربوط
        if (patient.UserId is not null)
        {
            var linked = await _users.FindByIdAsync(patient.UserId);
            if (linked is not null && await _users.IsInRoleAsync(linked, Roles.Patient))
                return (patient, linked, AccountState.Linked);

            return (patient, null, AccountState.None);
        }

        // معلّق: حساب اسم الدخول بتاعه هو موبايل المريض، ومش مربوط بحد
        var pending = await _users.FindByNameAsync(patient.Phone);

        if (pending is not null &&
            await _users.IsInRoleAsync(pending, Roles.Patient) &&
            !await _uow.Patients.AnyAsync(p => p.UserId == pending.Id))
        {
            return (patient, pending, AccountState.Pending);
        }

        return (patient, null, AccountState.None);
    }
}