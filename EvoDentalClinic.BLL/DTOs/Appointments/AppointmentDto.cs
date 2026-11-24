using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.Appointments
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = null!;
        public string PatientPhone { get; set; } = null!;
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
