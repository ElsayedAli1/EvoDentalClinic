using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;
using EvoDentalClinic.BLL.DTOs.PatientDto;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly IUnitOfWork _uow;
    private readonly IPatientService _patients;
    private readonly IClinicClock _clock;

    public AccountService(
        UserManager<ApplicationUser> users,
        IUnitOfWork uow,
        IPatientService patients,
        IClinicClock clock)
    {
        _users = users;
        _uow = uow;
        _patients = patients;
        _clock = clock;
    }

    // اللي كتبه المستخدم في خانة الدخول (إيميل أو موبايل) → اسم الدخول الفعلي
    public async Task<string?> ResolveUserNameAsync(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        identifier = identifier.Trim();

        if (identifier.Contains('@'))
        {
            var normalized = _users.NormalizeEmail(identifier);
            return await _users.Users
                .Where(u => u.NormalizedEmail == normalized)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }

        return PhoneHelper.Normalize(identifier);
    }

    public async Task<Result<RegistrationOutcome>> RegisterAsync(RegisterPatientDto dto)
    {
        var name = dto.FullName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Fail("الاسم مطلوب");

        if (name.Length > 150)
            return Fail("الاسم طويل جداً");

        var phone = PhoneHelper.Normalize(dto.Phone);
        if (phone is null)
            return Fail("رقم الموبايل غير صحيح");

        if (dto.DateOfBirth is not null && dto.DateOfBirth > _clock.Today)
            return Fail("تاريخ الميلاد لا يمكن أن يكون في المستقبل");

        var email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        if (email is not null)
        {
            if (!IdentityHelpers.IsValidEmail(email))
                return Fail("الإيميل غير صحيح");

            if (await _users.IsEmailTakenAsync(email))
                return Fail("الإيميل ده مستخدم قبل كده");
        }

        // الموبايل هو اسم الدخول، فلو فيه حساب بنفس الرقم يبقى متسجل
        const string phoneTaken = "الرقم ده عليه حساب بالفعل، سجّل دخول بيه";

        if (await _users.FindByNameAsync(phone) is not null)
            return Fail(phoneTaken);

        var existing = (await _uow.Patients.FindAsync(p => p.Phone == phone)).FirstOrDefault();
        if (existing?.UserId is not null)
            return Fail(phoneTaken);

        var user = new ApplicationUser
        {
            UserName = phone,
            PhoneNumber = phone,
            Email = email,
            FullName = name
        };

        var created = await _users.CreateAsync(user, dto.Password);
        if (!created.Succeeded)
            return Fail(created.ToMessage());

        var roleResult = await _users.AddToRoleAsync(user, Roles.Patient);
        if (!roleResult.Succeeded)
        {
            await _users.DeleteAsync(user);   // ما نسيبش حساب من غير دور
            return Fail(roleResult.ToMessage());
        }

        // الرقم مسجل في العيادة: مش بنربط تلقائياً، الاستقبال هو اللي بيربط
        if (existing is not null)
            return Result<RegistrationOutcome>.Success(RegistrationOutcome.PendingLink);

        try
        {
            var patient = await _patients.CreateAsync(new CreatePatientDto
            {
                FullName = name,
                Phone = phone,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                UserId = user.Id
            });

            if (!patient.IsSuccess)
            {
                await _users.DeleteAsync(user);
                return Fail(patient.Error!);
            }
        }
        catch
        {
            await _users.DeleteAsync(user);
            throw;
        }

        return Result<RegistrationOutcome>.Success(RegistrationOutcome.Active);
    }
    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure("الحساب غير موجود");

        if (currentPassword == newPassword)
            return Result.Failure("كلمة المرور الجديدة لازم تختلف عن الحالية");

        // الباسورد الغلط بيتعدّ في محاولات القفل، فجلسة مسروقة ما تقدرش تخمّن الباسورد
        if (!await _users.CheckPasswordAsync(user, currentPassword))
        {
            await _users.AccessFailedAsync(user);
            return Result.Failure("كلمة المرور الحالية غير صحيحة");
        }

        var result = await _users.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return Result.Failure(result.ToMessage());

        await _users.ResetAccessFailedCountAsync(user);
        return Result.Success();
    }
    private static Result<RegistrationOutcome> Fail(string error)
        => Result<RegistrationOutcome>.Failure(error);
}