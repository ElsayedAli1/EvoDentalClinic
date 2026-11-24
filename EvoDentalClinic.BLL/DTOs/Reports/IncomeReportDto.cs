namespace EvoDentalClinic.BLL.DTOs.Reports;

public class IncomeReportDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public List<DailyIncomeDto> Days { get; set; } = new();

    public decimal Total => Days.Sum(d => d.Amount);
    public int PaymentsCount => Days.Sum(d => d.Count);

    // إحصائيات الحجوزات في نفس الفترة
    public int BookedCount { get; set; }
    public int CompletedCount { get; set; }
    public int NoShowCount { get; set; }
    public int CancelledCount { get; set; }
}

public class DailyIncomeDto
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}