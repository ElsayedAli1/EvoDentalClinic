using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.Treatments
{
    public class CreateTreatmentRecordDto
    {
        public int PatientId { get; set; }
        public DateOnly Date { get; set; }
        public int ToothNumber { get; set; }
        public string Procedure { get; set; } = null!;
        public decimal TotalCost { get; set; }
    }
}
