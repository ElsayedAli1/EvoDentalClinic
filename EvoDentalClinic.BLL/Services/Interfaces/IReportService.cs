using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Reports;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IReportService
{
    Task<Result<IncomeReportDto>> GetIncomeAsync(DateOnly fromDate, DateOnly toDate);
    Task<List<OutstandingPatientDto>> GetOutstandingAsync();
}