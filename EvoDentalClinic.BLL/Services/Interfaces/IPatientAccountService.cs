using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Account;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IPatientAccountService
{
    Task<PatientAccountDto> GetAsync(int patientId);
    Task<Result> LinkAsync(int patientId);
    Task<Result> ResetPasswordAsync(int patientId, string newPassword);
    Task<Result> DeleteAsync(int patientId);
}