using AutoMapper;
using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Payments;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Implemintation
{

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetByTreatmentAsync(int treatmentRecordId)
        {
            var payments = await _uow.Payments.FindAsync(
                p => p.TreatmentRecordId == treatmentRecordId);

            return _mapper.Map<IEnumerable<PaymentDto>>(payments.OrderBy(p => p.PaidAt));
        }

        public async Task<Result<PaymentDto>> AddAsync(CreatePaymentDto dto)
        {
            if (dto.Amount <= 0)
                return Result<PaymentDto>.Failure("المبلغ لازم يكون أكبر من صفر");

            if (dto.Amount != Math.Round(dto.Amount, 2))
                return Result<PaymentDto>.Failure("المبلغ لازم يكون بحد أقصى رقمين بعد العلامة العشرية");

            if (dto.Note?.Length > 250)
                return Result<PaymentDto>.Failure("الملاحظة طويلة جداً (الحد الأقصى 250 حرف)");

            var records = await _uow.TreatmentRecords.FindAsync(
                t => t.Id == dto.TreatmentRecordId, t => t.Payments);
            var record = records.FirstOrDefault();

            if (record is null)
                return Result<PaymentDto>.Failure("الإجراء غير موجود");

            var balance = record.TotalCost - record.Payments.Sum(p => p.Amount);

            if (balance <= 0)
                return Result<PaymentDto>.Failure("الإجراء ده مسدد بالكامل");

            if (dto.Amount > balance)
                return Result<PaymentDto>.Failure($"المبلغ أكبر من المتبقي ({balance})");

            var payment = _mapper.Map<Payment>(dto);
            payment.PaidAt = DateTime.UtcNow;

            await _uow.Payments.AddAsync(payment);
            await _uow.SaveChangesAsync();

            return Result<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
        }
    }
}
