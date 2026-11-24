using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Treatments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Implemintation
{

    public class TreatmentService : ITreatmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IClinicClock _clock;

        public TreatmentService(IUnitOfWork uow, IMapper mapper, IClinicClock clock)
        {
            _uow = uow;
            _mapper = mapper;
            _clock = clock;
        }

        public async Task<TreatmentRecordDto?> GetByIdAsync(int id)
        {
            var record = await GetWithPaymentsAsync(id);
            return record is null ? null : _mapper.Map<TreatmentRecordDto>(record);
        }

        public async Task<IEnumerable<TreatmentRecordDto>> GetByPatientAsync(int patientId)
        {
            var records = await _uow.TreatmentRecords.FindAsync(
                t => t.PatientId == patientId, t => t.Payments);

            return _mapper.Map<IEnumerable<TreatmentRecordDto>>(
                records.OrderByDescending(t => t.Date).ThenByDescending(t => t.Id));
        }

        public async Task<Result<TreatmentRecordDto>> CreateAsync(CreateTreatmentRecordDto dto)
        {
            var error = Validate(dto.Date, dto.ToothNumber, dto.Procedure, dto.TotalCost);
            if (error is not null)
                return Result<TreatmentRecordDto>.Failure(error);

            if (!await _uow.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return Result<TreatmentRecordDto>.Failure("المريض غير موجود");

            var record = _mapper.Map<TreatmentRecord>(dto);
            record.Procedure = dto.Procedure.Trim();

            await _uow.TreatmentRecords.AddAsync(record);
            await _uow.SaveChangesAsync();

            return Result<TreatmentRecordDto>.Success(_mapper.Map<TreatmentRecordDto>(record));
        }

        public async Task<Result> UpdateAsync(UpdateTreatmentRecordDto dto)
        {
            var record = await GetWithPaymentsAsync(dto.Id);
            if (record is null)
                return Result.Failure("الإجراء غير موجود");

            var error = Validate(dto.Date, dto.ToothNumber, dto.Procedure, dto.TotalCost);
            if (error is not null)
                return Result.Failure(error);

            var paid = record.Payments.Sum(p => p.Amount);
            if (dto.TotalCost < paid)
                return Result.Failure($"التكلفة أقل من المبلغ المدفوع فعلاً ({paid})");

            _mapper.Map(dto, record);
            record.Procedure = dto.Procedure.Trim();

            _uow.TreatmentRecords.Update(record);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        // ---------- دوال مساعدة ----------

        private async Task<TreatmentRecord?> GetWithPaymentsAsync(int id)
        {
            var records = await _uow.TreatmentRecords.FindAsync(
                t => t.Id == id, t => t.Payments);

            return records.FirstOrDefault();
        }

        private string? Validate(DateOnly date, int toothNumber, string? procedure, decimal totalCost)
        {
            if (!FdiTooth.IsValid(toothNumber))
                return "رقم السن غير صحيح";

            if (string.IsNullOrWhiteSpace(procedure))
                return "اسم الإجراء مطلوب";

            if (procedure.Trim().Length > 200)
                return "اسم الإجراء طويل جداً (الحد الأقصى 200 حرف)";

            if (totalCost <= 0)
                return "التكلفة لازم تكون أكبر من صفر";

            if (date > _clock.Today)
                return "تاريخ الإجراء لا يمكن أن يكون في المستقبل";

            return null;
        }
    }

}
