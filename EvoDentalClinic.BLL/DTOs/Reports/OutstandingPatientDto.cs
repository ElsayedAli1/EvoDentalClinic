namespace EvoDentalClinic.BLL.DTOs.Reports;

public class OutstandingPatientDto
{
    public int PatientId { get; set; }
    public string Code { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime? LastPaymentAt { get; set; }   // UTC

    public decimal Balance => TotalCost - TotalPaid;
}