using EvoDentalClinic.BLL.DTOs.Clinic;
using EvoDentalClinic.BLL.DTOs.Reviews;

namespace EvoDentalClinic.PL.ViewModels;

public class LandingViewModel
{
    public ClinicSettingsDto? Settings { get; set; }
    public List<string> ClinicImages { get; set; } = new();
    public string? DoctorImage { get; set; }
    public List<string> CaseImages { get; set; } = new();
    public ReviewSummaryDto? Reviews { get; set; }

}