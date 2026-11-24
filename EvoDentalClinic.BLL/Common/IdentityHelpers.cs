using System.Net.Mail;
using EvoDentalClinic.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvoDentalClinic.BLL.Common;

public static class IdentityHelpers
{
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try { return new MailAddress(email).Address == email; }
        catch (FormatException) { return false; }
    }

    public static async Task<bool> IsEmailTakenAsync(
        this UserManager<ApplicationUser> users, string email)
    {
        var normalized = users.NormalizeEmail(email);
        return await users.Users.AnyAsync(u => u.NormalizedEmail == normalized);
    }

    public static string ToMessage(this IdentityResult result)
        => string.Join("، ", result.Errors.Select(Translate).Distinct());

    private static string Translate(IdentityError e) => e.Code switch
    {
        "DuplicateUserName" => "الرقم ده عليه حساب بالفعل",
        "DuplicateEmail" => "الإيميل ده مستخدم قبل كده",
        "InvalidEmail" or "InvalidUserName" => "البيانات المدخلة غير صحيحة",
        "PasswordTooShort" => "كلمة المرور لازم تكون 8 حروف على الأقل",
        "PasswordRequiresDigit" => "كلمة المرور لازم تحتوي على رقم",
        _ => e.Description
    };
}