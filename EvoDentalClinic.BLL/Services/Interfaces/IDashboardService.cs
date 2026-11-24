using EvoDentalClinic.BLL.DTOs.Dashboard;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(bool includeFinance);
}