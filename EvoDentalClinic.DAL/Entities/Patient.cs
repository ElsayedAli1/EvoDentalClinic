using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string Phone { get; set; } = null!;
        public string? Address { get; set; }
        public string? Job { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public MedicalHistory? MedicalHistory { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<TreatmentRecord> TreatmentRecords { get; set; } = new List<TreatmentRecord>();
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
