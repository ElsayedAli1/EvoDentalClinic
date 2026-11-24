using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Enums;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Implemintation
{
    public class AppointmentService : IAppointmentService
    {
        private const int MaxActiveOnlineBookings = 2;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IClinicClock _clock;

        public AppointmentService(IUnitOfWork uow, IMapper mapper, IClinicClock clock)
        {
            _uow = uow;
            _mapper = mapper;
            _clock = clock;
        }

        // ---------- المواعيد الفاضية ----------

        public async Task<IEnumerable<TimeOnly>> GetAvailableSlotsAsync(DateOnly date)
        {
            if (date < _clock.Today) return Enumerable.Empty<TimeOnly>();

            var settings = await GetSettingsAsync();
            if (!IsWorkDay(settings, date)) return Enumerable.Empty<TimeOnly>();

            var booked = await _uow.Appointments.FindAsync(a =>
                a.Date == date && a.Status != AppointmentStatus.Cancelled);
            var bookedTimes = booked.Select(a => a.StartTime).ToHashSet();

            var slots = GenerateSlots(settings).Where(s => !bookedTimes.Contains(s));

            if (date == _clock.Today)
                slots = slots.Where(s => s > _clock.TimeNow);

            return slots.ToList();
        }

        // ---------- العرض ----------

        public async Task<IEnumerable<AppointmentDto>> GetByDateAsync(DateOnly date)
        {
            var appointments = await _uow.Appointments.FindAsync(
                a => a.Date == date, a => a.Patient);

            return _mapper.Map<IEnumerable<AppointmentDto>>(
                appointments.OrderBy(a => a.StartTime));
        }

        public async Task<IEnumerable<AppointmentDto>> GetByPatientAsync(int patientId)
        {
            var appointments = await _uow.Appointments.FindAsync(
                a => a.PatientId == patientId, a => a.Patient);

            return _mapper.Map<IEnumerable<AppointmentDto>>(
                appointments.OrderByDescending(a => a.Date).ThenByDescending(a => a.StartTime));
        }

        // ---------- الحجز ----------

        public async Task<Result<AppointmentDto>> BookAsync(CreateAppointmentDto dto)
        {
            if (dto.Notes?.Length > 500)
                return Result<AppointmentDto>.Failure("الملاحظات طويلة جداً (الحد الأقصى 500 حرف)");

            var patient = await _uow.Patients.GetByIdAsync(dto.PatientId);
            if (patient is null)
                return Result<AppointmentDto>.Failure("المريض غير موجود");

            if (dto.Date < _clock.Today)
                return Result<AppointmentDto>.Failure("لا يمكن الحجز في تاريخ سابق");

            var settings = await GetSettingsAsync();

            if (!IsWorkDay(settings, dto.Date))
                return Result<AppointmentDto>.Failure("العيادة مغلقة في اليوم ده");

            if (!GenerateSlots(settings).Contains(dto.StartTime))
                return Result<AppointmentDto>.Failure("الميعاد ده مش من مواعيد العيادة");

            if (dto.Date == _clock.Today && dto.StartTime <= _clock.TimeNow)
                return Result<AppointmentDto>.Failure("الميعاد ده عدّى، اختار ميعاد تاني");

            if (await IsSlotTakenAsync(dto.Date, dto.StartTime))
                return Result<AppointmentDto>.Failure("الميعاد ده اتحجز، اختار ميعاد تاني");

            var appointment = _mapper.Map<Appointment>(dto);
            appointment.Status = AppointmentStatus.Booked;
            appointment.Patient = patient;

            await _uow.Appointments.AddAsync(appointment);

            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // لو حد حجز نفس الميعاد في نفس اللحظة، الـ Unique Index في الداتا بيز بيرفض
                if (await IsSlotTakenAsync(dto.Date, dto.StartTime))
                    return Result<AppointmentDto>.Failure("الميعاد ده اتحجز لتوّه، اختار ميعاد تاني");

                throw; // سبب تاني، منخفيهوش
            }

            return Result<AppointmentDto>.Success(_mapper.Map<AppointmentDto>(appointment));
        }

        // ---------- الإلغاء ----------

        public async Task<Result> CancelAsync(int appointmentId)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(appointmentId);
            if (appointment is null)
                return Result.Failure("الحجز غير موجود");

            if (appointment.Status != AppointmentStatus.Booked)
                return Result.Failure("مينفعش تلغي حجز مش في حالة \"محجوز\"");

            appointment.Status = AppointmentStatus.Cancelled;
            _uow.Appointments.Update(appointment);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result> SetStatusAsync(int appointmentId, AppointmentStatus status)
        {
            if (status != AppointmentStatus.Completed && status != AppointmentStatus.NoShow)
                return Result.Failure("الحالة دي مش مسموحة من هنا");

            var appointment = await _uow.Appointments.GetByIdAsync(appointmentId);
            if (appointment is null)
                return Result.Failure("الحجز غير موجود");

            if (appointment.Status != AppointmentStatus.Booked)
                return Result.Failure("الحالة بتتغير للحجوزات المحجوزة بس");

            var started = appointment.Date < _clock.Today ||
                          (appointment.Date == _clock.Today && appointment.StartTime <= _clock.TimeNow);

            if (!started)
                return Result.Failure("الميعاد لسه ما جاش");

            appointment.Status = status;
            _uow.Appointments.Update(appointment);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        // ---------- دوال مساعدة ----------

        private async Task<ClinicSettings> GetSettingsAsync()
            => await _uow.ClinicSettings.GetByIdAsync(1)
               ?? throw new InvalidOperationException("إعدادات العيادة غير موجودة في الداتا بيز");

        private async Task<bool> IsSlotTakenAsync(DateOnly date, TimeOnly time)
            => await _uow.Appointments.AnyAsync(a =>
                a.Date == date &&
                a.StartTime == time &&
                a.Status != AppointmentStatus.Cancelled);

        private static bool IsWorkDay(ClinicSettings settings, DateOnly date)
    => WorkDaysParser.Parse(settings.WorkDays).Contains(date.DayOfWeek);

        private static List<TimeOnly> GenerateSlots(ClinicSettings settings)
      => SlotGenerator.Generate(settings.OpenTime, settings.CloseTime, settings.SlotMinutes);
        public async Task<Result<AppointmentDto>> BookOnlineAsync(CreateAppointmentDto dto)
        {
            var upcoming = await _uow.Appointments.FindAsync(a =>
                a.PatientId == dto.PatientId &&
                a.Status == AppointmentStatus.Booked &&
                a.Date >= _clock.Today);

            var active = upcoming.Count(a => a.Date > _clock.Today || a.StartTime > _clock.TimeNow);

            if (active >= MaxActiveOnlineBookings)
                return Result<AppointmentDto>.Failure(
                    $"عندك {active} حجوزات قادمة. ألغِ واحد منهم أو كلّم العيادة لو محتاج حجز تاني");

            return await BookAsync(dto);
        }

        public async Task<Result> CancelOwnAsync(int appointmentId, int patientId)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(appointmentId);

            // نفس الرسالة للحجز غير الموجود وحجز مريض تاني، عشان ما نكشفش وجوده
            if (appointment is null || appointment.PatientId != patientId)
                return Result.Failure("الحجز غير موجود");

            var started = appointment.Date < _clock.Today ||
                          (appointment.Date == _clock.Today && appointment.StartTime <= _clock.TimeNow);

            if (started)
                return Result.Failure("الميعاد ده جه أو عدّى، كلّم العيادة لو محتاج تعدّله");

            return await CancelAsync(appointmentId);
        }
    }
}
