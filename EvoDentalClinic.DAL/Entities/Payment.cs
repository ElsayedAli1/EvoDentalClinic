using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int TreatmentRecordId { get; set; }
        public TreatmentRecord TreatmentRecord { get; set; } = null!;

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Note { get; set; }
    }
}
