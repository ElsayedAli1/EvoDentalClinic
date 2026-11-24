using EvoDentalClinic.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.PatientDto
{
    public class PatientDto
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

        public int? Age
        {
            get
            {
                if (DateOfBirth is null) return null;

                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
