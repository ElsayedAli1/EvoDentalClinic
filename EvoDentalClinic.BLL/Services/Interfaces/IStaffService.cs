using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Staff;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IStaffService
{
    Task<IEnumerable<StaffDto>> GetAllAsync();
    Task<StaffDto?> GetByIdAsync(string id);
    Task<Result> CreateReceptionistAsync(CreateStaffDto dto);
    Task<Result> SetActiveAsync(string id, bool active);
    Task<Result> ResetPasswordAsync(string id, string newPassword);
}