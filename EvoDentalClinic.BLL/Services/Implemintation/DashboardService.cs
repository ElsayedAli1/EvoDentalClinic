using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Dashboard;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly IAppointmentService _appointments;
    private readonly IClinicClock _clock;

    public DashboardService(
        IUnitOfWork uow,
        IAppointmentService appointments,
        IClinicClock clock)
    {
        _uow = uow;
        _appointments = appointments;
        _clock = clock;
    }

    public async Task<DashboardDto> GetAsync(bool includeFinance)
    {
        var today = _clock.Today;

        var dto = new DashboardDto
        {
            Date = today,
            Appointments = (await _appointments.GetByDateAsync(today)).ToList()
        };

        if (includeFinance)
        {
            dto.Finance = await BuildFinanceAsync(today);
            dto.PendingReviews = (await _uow.Reviews.FindAsync(r => r.Status == ReviewStatus.Pending)).Count();
        }
        return dto;
    }

    private async Task<DashboardFinanceDto> BuildFinanceAsync(DateOnly today)
    {
        // إيراد النهاردة: بنجيب دفعات آخر يومين (نافذة واسعة بتغطي فرق التوقيت)
        // وبنفلترها بتاريخ القاهرة في الذاكرة
        var since = DateTime.UtcNow.AddDays(-2);
        var recent = await _uow.Payments.FindAsync(p => p.PaidAt >= since);
        var incomeToday = recent
            .Where(p => _clock.ToClinicDate(p.PaidAt) == today)
            .Sum(p => p.Amount);

        // المتبقي: لكل مريض، مجموع (التكلفة - المدفوع) لإجراءاته
        var records = await _uow.TreatmentRecords.GetAllAsync(t => t.Payments);
        var balances = records
            .GroupBy(t => t.PatientId)
            .Select(g => g.Sum(t => t.TotalCost - t.Payments.Sum(p => p.Amount)))
            .Where(b => b > 0)
            .ToList();

        return new DashboardFinanceDto
        {
            IncomeToday = incomeToday,
            OutstandingBalance = balances.Sum(),
            PatientsWithBalance = balances.Count
        };
    }
}