using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.PatientDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto?> GetByIdAsync(int id);
        Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize = 20);
        Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto);
        Task<Result> UpdateAsync(UpdatePatientDto dto);
        Task<PatientDto?> GetByUserIdAsync(string userId);
    }
}
