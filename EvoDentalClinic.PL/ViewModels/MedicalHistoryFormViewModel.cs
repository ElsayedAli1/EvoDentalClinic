using EvoDentalClinic.BLL.DTOs.MedicalHistories;

namespace EvoDentalClinic.PL.ViewModels;

public class MedicalHistoryFormViewModel
{
    public string? PatientName { get; set; }
    public bool IsFemale { get; set; }
    public MedicalHistoryDto History { get; set; } = new();
}

public class MedicalHistoryDetailsViewModel
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public MedicalHistoryDto? History { get; set; }
}