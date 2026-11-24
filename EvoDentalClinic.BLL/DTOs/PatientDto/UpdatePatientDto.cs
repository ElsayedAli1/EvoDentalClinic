using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.PatientDto
{
    public class UpdatePatientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string Phone { get; set; } = null!;
        public string? Address { get; set; }
        public string? Job { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
    }
}
