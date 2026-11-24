using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IAccountService
{
    Task<string?> ResolveUserNameAsync(string? identifier);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<Result<RegistrationOutcome>> RegisterAsync(RegisterPatientDto dto);
}