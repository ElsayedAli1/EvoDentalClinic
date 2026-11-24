using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class TreatmentRecord
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public DateOnly Date { get; set; }
        public int ToothNumber { get; set; }
        public string Procedure { get; set; } = null!;
        public decimal TotalCost { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
