using EvoDentalClinic.BLL.DTOs.Account;
using EvoDentalClinic.BLL.DTOs.Appointments;
using EvoDentalClinic.BLL.DTOs.PatientDto;
using EvoDentalClinic.BLL.DTOs.Treatments;

namespace EvoDentalClinic.PL.ViewModels;

public class PatientFileViewModel
{
    public PatientDto Patient { get; set; } = null!;
    public List<AppointmentDto> Appointments { get; set; } = new();
    public List<TreatmentRecordDto> Treatments { get; set; } = new();

    public decimal TotalCost => Treatments.Sum(t => t.TotalCost);
    public decimal TotalPaid => Treatments.Sum(t => t.TotalPaid);
    public decimal Balance => TotalCost - TotalPaid;
    public PatientAccountDto Account { get; set; } = new();
}