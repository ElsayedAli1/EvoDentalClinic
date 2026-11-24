using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.BLL.DTOs.Dashboard;

public class DashboardDto
{
    public DateOnly Date { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = new();

    // null للاستقبال
    public DashboardFinanceDto? Finance { get; set; }
    public int? PendingReviews { get; set; }   // للدكتور بس
    public int TotalCount => Appointments.Count(a => a.Status != AppointmentStatus.Cancelled);
    public int WaitingCount => Appointments.Count(a => a.Status == AppointmentStatus.Booked);
    public int CompletedCount => Appointments.Count(a => a.Status == AppointmentStatus.Completed);
    public int NoShowCount => Appointments.Count(a => a.Status == AppointmentStatus.NoShow);

    public AppointmentDto? Next
        => Appointments.FirstOrDefault(a => a.Status == AppointmentStatus.Booked);
}

public class DashboardFinanceDto
{
    public decimal IncomeToday { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int PatientsWithBalance { get; set; }
}