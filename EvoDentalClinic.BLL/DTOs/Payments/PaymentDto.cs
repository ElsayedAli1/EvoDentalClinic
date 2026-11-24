using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int TreatmentRecordId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Note { get; set; }
    }
}
