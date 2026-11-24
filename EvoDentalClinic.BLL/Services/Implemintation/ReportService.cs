using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Reports;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class ReportService : IReportService
{
    private const int MaxRangeDays = 366;

    private readonly IUnitOfWork _uow;
    private readonly IClinicClock _clock;

    public ReportService(IUnitOfWork uow, IClinicClock clock)
    {
        _uow = uow;
        _clock = clock;
    }

    public async Task<Result<IncomeReportDto>> GetIncomeAsync(DateOnly fromDate, DateOnly toDate)
    {
        if (fromDate > toDate)
            return Result<IncomeReportDto>.Failure("تاريخ البداية لازم يكون قبل تاريخ النهاية");

        if (toDate.DayNumber - fromDate.DayNumber > MaxRangeDays)
            return Result<IncomeReportDto>.Failure("أقصى فترة للتقرير سنة واحدة");

        // الدفعات متخزنة UTC. بنجيب نافذة واسعة (يوم قبل ويومين بعد) بتغطي فرق التوقيت،
        // وبعدين نفلتر بتاريخ القاهرة في الذاكرة
        var since = fromDate.ToDateTime(TimeOnly.MinValue).AddDays(-1);
        var until = toDate.ToDateTime(TimeOnly.MinValue).AddDays(2);

        var candidates = await _uow.Payments.FindAsync(p => p.PaidAt >= since && p.PaidAt < until);

        var days = candidates
            .Select(p => new { Date = _clock.ToClinicDate(p.PaidAt), p.Amount })
            .Where(x => x.Date >= fromDate && x.Date <= toDate)
            .GroupBy(x => x.Date)
            .Select(g => new DailyIncomeDto
            {
                Date = g.Key,
                Amount = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .OrderByDescending(d => d.Date)
            .ToList();

        var appointments = (await _uow.Appointments.FindAsync(a =>
            a.Date >= fromDate && a.Date <= toDate)).ToList();

        return Result<IncomeReportDto>.Success(new IncomeReportDto
        {
            From = fromDate,
            To = toDate,
            Days = days,
            BookedCount = appointments.Count(a => a.Status == AppointmentStatus.Booked),
            CompletedCount = appointments.Count(a => a.Status == AppointmentStatus.Completed),
            NoShowCount = appointments.Count(a => a.Status == AppointmentStatus.NoShow),
            CancelledCount = appointments.Count(a => a.Status == AppointmentStatus.Cancelled)
        });
    }

    public async Task<List<OutstandingPatientDto>> GetOutstandingAsync()
    {
        var records = await _uow.TreatmentRecords.GetAllAsync(t => t.Payments);

        var perPatient = records
            .GroupBy(t => t.PatientId)
            .Select(g => new
            {
                PatientId = g.Key,
                Total = g.Sum(t => t.TotalCost),
                Paid = g.Sum(t => t.Payments.Sum(p => p.Amount)),
                LastPayment = g.SelectMany(t => t.Payments).Select(p => (DateTime?)p.PaidAt).Max()
            })
            .Where(x => x.Total - x.Paid > 0)
            .ToList();

        if (perPatient.Count == 0)
            return new List<OutstandingPatientDto>();

        var ids = perPatient.Select(x => x.PatientId).ToList();
        var patients = (await _uow.Patients.FindAsync(p => ids.Contains(p.Id)))
            .ToDictionary(p => p.Id);

        return perPatient
            .Where(x => patients.ContainsKey(x.PatientId))
            .Select(x => new OutstandingPatientDto
            {
                PatientId = x.PatientId,
                Code = patients[x.PatientId].Code,
                FullName = patients[x.PatientId].FullName,
                Phone = patients[x.PatientId].Phone,
                TotalCost = x.Total,
                TotalPaid = x.Paid,
                LastPaymentAt = x.LastPayment
            })
            .OrderByDescending(x => x.Balance)
            .ToList();
    }
}