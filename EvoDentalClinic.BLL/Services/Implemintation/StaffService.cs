using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Staff;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class StaffService : IStaffService
{
    private readonly UserManager<ApplicationUser> _users;

    public StaffService(UserManager<ApplicationUser> users)
    {
        _users = users;
    }

    public async Task<IEnumerable<StaffDto>> GetAllAsync()
    {
        var result = new List<StaffDto>();

        foreach (var role in new[] { Roles.Doctor, Roles.Receptionist })
        {
            var inRole = await _users.GetUsersInRoleAsync(role);
            result.AddRange(inRole.Select(u => ToDto(u, role)));
        }

        return result
            .OrderBy(s => s.Role == Roles.Doctor ? 0 : 1)
            .ThenBy(s => s.FullName)
            .ToList();
    }

    public async Task<StaffDto?> GetByIdAsync(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _users.GetRolesAsync(user);

        var role = roles.Contains(Roles.Doctor) ? Roles.Doctor
                 : roles.Contains(Roles.Receptionist) ? Roles.Receptionist
                 : null;

        // مرضى الحجز الأونلاين مش موظفين، فمش بيظهروا هنا
        return role is null ? null : ToDto(user, role);
    }

    public async Task<Result> CreateReceptionistAsync(CreateStaffDto dto)
    {
        var name = dto.FullName?.Trim();
        var email = dto.Email?.Trim();
        if (!IdentityHelpers.IsValidEmail(email))
            return Result.Failure("الإيميل غير صحيح");

        if (await _users.IsEmailTakenAsync(email))
            return Result.Failure("الإيميل ده مستخدم قبل كده");
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("الاسم مطلوب");

        if (name.Length > 100)
            return Result.Failure("الاسم طويل جداً");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("الإيميل مطلوب");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name,
            EmailConfirmed = true
        };

        var created = await _users.CreateAsync(user, dto.Password);
        if (!created.Succeeded)
            return Failure(created);

        var roleResult = await _users.AddToRoleAsync(user, Roles.Receptionist);
        if (!roleResult.Succeeded)
        {
            await _users.DeleteAsync(user);   // ما نسيبش حساب من غير دور
            return Failure(roleResult);
        }

        return Result.Success();
    }

    public async Task<Result> SetActiveAsync(string id, bool active)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null)
            return Result.Failure("الحساب غير موجود");

        if (await _users.IsInRoleAsync(user, Roles.Doctor))
            return Result.Failure("مينفعش توقف حساب الدكتور");

        if (!await _users.IsInRoleAsync(user, Roles.Receptionist))
            return Result.Failure("الحساب ده مش حساب موظف");

        if (active)
        {
            await _users.SetLockoutEndDateAsync(user, null);
            await _users.ResetAccessFailedCountAsync(user);
        }
        else
        {
            await _users.SetLockoutEnabledAsync(user, true);
            await _users.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            await _users.UpdateSecurityStampAsync(user);   // بيبوّظ الجلسات المفتوحة
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string id, string newPassword)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null)
            return Result.Failure("الحساب غير موجود");

        if (!await _users.IsInRoleAsync(user, Roles.Receptionist))
            return Result.Failure("تغيير الباسورد من هنا لحسابات الاستقبال بس");

        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var result = await _users.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded ? Result.Success() : Failure(result);
    }

    // ---------- دوال مساعدة ----------

    private static StaffDto ToDto(ApplicationUser user, string role) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email ?? "",
        Role = role,
        IsActive = user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow
    };

    private static Result Failure(IdentityResult result)
        => Result.Failure(string.Join("، ", result.Errors.Select(Translate)));

    // رسائل Identity بالإنجليزي، فبنترجم الشائع منها
    private static string Translate(IdentityError e) => e.Code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "الإيميل ده مستخدم قبل كده",
        "InvalidEmail" or "InvalidUserName" => "الإيميل غير صحيح",
        "PasswordTooShort" => "كلمة المرور لازم تكون 8 حروف على الأقل",
        "PasswordRequiresDigit" => "كلمة المرور لازم تحتوي على رقم",
        _ => e.Description
    };
}