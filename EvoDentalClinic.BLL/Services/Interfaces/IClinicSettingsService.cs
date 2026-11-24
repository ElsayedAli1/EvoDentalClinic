using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Clinic;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IClinicSettingsService
{
    Task<ClinicSettingsDto?> GetAsync();
    Task<Result> UpdateAsync(ClinicSettingsDto dto);
}