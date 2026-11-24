namespace EvoDentalClinic.BLL.DTOs.Payments;

public class CreatePaymentDto
{
    public int TreatmentRecordId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}