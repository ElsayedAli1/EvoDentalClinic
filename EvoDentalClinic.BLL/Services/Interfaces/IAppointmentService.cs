using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<TimeOnly>> GetAvailableSlotsAsync(DateOnly date);
        Task<IEnumerable<AppointmentDto>> GetByDateAsync(DateOnly date);
        Task<IEnumerable<AppointmentDto>> GetByPatientAsync(int patientId);
        Task<Result<AppointmentDto>> BookAsync(CreateAppointmentDto dto);
        Task<Result> CancelAsync(int appointmentId);
        Task<Result> SetStatusAsync(int appointmentId, AppointmentStatus status);
        Task<Result<AppointmentDto>> BookOnlineAsync(CreateAppointmentDto dto);
        Task<Result> CancelOwnAsync(int appointmentId, int patientId);
    }
}
