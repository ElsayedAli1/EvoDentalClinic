using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Treatments;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<TreatmentRecordDto?> GetByIdAsync(int id);
        Task<IEnumerable<TreatmentRecordDto>> GetByPatientAsync(int patientId);
        Task<Result<TreatmentRecordDto>> CreateAsync(CreateTreatmentRecordDto dto);
        Task<Result> UpdateAsync(UpdateTreatmentRecordDto dto);
    }
}
