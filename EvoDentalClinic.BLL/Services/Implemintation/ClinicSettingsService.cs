using System.Globalization;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Clinic;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;

namespace EvoDentalClinic.BLL.Services.Implementations;

public class ClinicSettingsService : IClinicSettingsService
{
    private static readonly int[] AllowedSlotMinutes = { 10, 15, 20, 30, 45, 60 };

    private readonly IUnitOfWork _uow;
    private readonly IClinicClock _clock;

    public ClinicSettingsService(IUnitOfWork uow, IClinicClock clock)
    {
        _uow = uow;
        _clock = clock;
    }

    public async Task<ClinicSettingsDto?> GetAsync()
    {
        var settings = await _uow.ClinicSettings.GetByIdAsync(1);
        if (settings is null)
            return null;

        return new ClinicSettingsDto
        {
            WorkDays = WorkDaysParser.Parse(settings.WorkDays),
            OpenTime = settings.OpenTime,
            CloseTime = settings.CloseTime,
            SlotMinutes = settings.SlotMinutes
        };
    }

    public async Task<Result> UpdateAsync(ClinicSettingsDto dto)
    {
        var days = dto.WorkDays.Distinct().ToList();

        if (days.Count == 0)
            return Result.Failure("اختار يوم شغل واحد على الأقل");

        if (dto.OpenTime >= dto.CloseTime)
            return Result.Failure("ميعاد الإغلاق لازم يكون بعد ميعاد الفتح");

        if (!AllowedSlotMinutes.Contains(dto.SlotMinutes))
            return Result.Failure("مدة الكشف لازم تكون من الاختيارات المتاحة");

        var slots = SlotGenerator.Generate(dto.OpenTime, dto.CloseTime, dto.SlotMinutes);
        if (slots.Count == 0)
            return Result.Failure("مدة الكشف أطول من ساعات الشغل");

        var settings = await _uow.ClinicSettings.GetByIdAsync(1);
        if (settings is null)
            return Result.Failure("إعدادات العيادة غير موجودة");

        // الحجوزات القادمة لازم تفضل متوافقة مع الإعدادات الجديدة
        var today = _clock.Today;
        var upcoming = await _uow.Appointments.FindAsync(a =>
            a.Status == AppointmentStatus.Booked && a.Date >= today);

        var daySet = days.ToHashSet();
        var slotSet = slots.ToHashSet();

        var conflicts = upcoming
            .Where(a => !daySet.Contains(a.Date.DayOfWeek) || !slotSet.Contains(a.StartTime))
            .ToList();

        if (conflicts.Count > 0)
        {
            var first = conflicts.Min(a => a.Date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return Result.Failure(
                $"فيه {conflicts.Count} حجز قادم مش هيتماشى مع الإعدادات الجديدة (أقربهم يوم {first}). " +
                "ألغِ الحجوزات دي الأول أو عدّل الإعدادات");
        }

        settings.WorkDays = WorkDaysParser.Format(days);
        settings.OpenTime = dto.OpenTime;
        settings.CloseTime = dto.CloseTime;
        settings.SlotMinutes = dto.SlotMinutes;

        _uow.ClinicSettings.Update(settings);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}