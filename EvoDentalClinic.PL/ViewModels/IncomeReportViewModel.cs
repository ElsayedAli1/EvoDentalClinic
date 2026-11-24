using EvoDentalClinic.BLL.DTOs.Reports;

namespace EvoDentalClinic.PL.ViewModels;

public class IncomeReportViewModel
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public IncomeReportDto? Report { get; set; }
    public string? Error { get; set; }

    // اختصارات الفترات
    public DateOnly Today { get; set; }
    public DateOnly MonthStart { get; set; }
    public DateOnly PrevMonthStart { get; set; }
    public DateOnly PrevMonthEnd { get; set; }
}