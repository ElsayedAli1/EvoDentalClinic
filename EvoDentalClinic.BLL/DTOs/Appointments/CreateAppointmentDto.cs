using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public int PatientId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Notes { get; set; }
    }
}