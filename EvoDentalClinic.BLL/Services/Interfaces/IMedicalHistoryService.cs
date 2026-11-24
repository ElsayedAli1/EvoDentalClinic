using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.MedicalHistories;

namespace EvoDentalClinic.BLL.Services.Interfaces;

public interface IMedicalHistoryService
{
    Task<MedicalHistoryDto?> GetByPatientAsync(int patientId);
    Task<Result> SaveAsync(MedicalHistoryDto dto);
}