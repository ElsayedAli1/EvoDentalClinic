using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetByTreatmentAsync(int treatmentRecordId);
        Task<Result<PaymentDto>> AddAsync(CreatePaymentDto dto);
    }
}
