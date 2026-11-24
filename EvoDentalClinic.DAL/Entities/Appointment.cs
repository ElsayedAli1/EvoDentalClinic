using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
