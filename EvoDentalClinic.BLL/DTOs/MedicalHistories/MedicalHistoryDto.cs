namespace EvoDentalClinic.BLL.DTOs.MedicalHistories;

public class MedicalHistoryDto
{
    public int PatientId { get; set; }

    public bool HasDiabetes { get; set; }
    public bool HasHeartDisease { get; set; }
    public bool HasBloodPressure { get; set; }
    public bool HasRespiratoryDisease { get; set; }
    public bool HasLiverDisease { get; set; }

    public bool HasDrugAllergy { get; set; }
    public string? DrugAllergyType { get; set; }

    public bool HadRecentDentalVisit { get; set; }
    public string? RecentVisitReason { get; set; }

    public bool HadPreviousExtraction { get; set; }
    public bool HadAnestheticAllergy { get; set; }
    public bool HadDelayedHealing { get; set; }
    public bool HadBleeding { get; set; }
    public bool HadFainting { get; set; }

    public bool HadPreviousProsthetics { get; set; }
    public string? ProstheticsType { get; set; }

    public bool HadRadiationOrChemo { get; set; }

    public bool IsPregnant { get; set; }
    public bool IsBreastfeeding { get; set; }

    public string? Notes { get; set; }
}