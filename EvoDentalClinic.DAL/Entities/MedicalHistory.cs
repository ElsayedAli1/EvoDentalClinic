using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class MedicalHistory
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        // أمراض مزمنة
        public bool HasDiabetes { get; set; }
        public bool HasHeartDisease { get; set; }
        public bool HasBloodPressure { get; set; }
        public bool HasRespiratoryDisease { get; set; }
        public bool HasLiverDisease { get; set; }

        // حساسية الأدوية
        public bool HasDrugAllergy { get; set; }
        public string? DrugAllergyType { get; set; }

        // زيارة دكتور أسنان قريباً
        public bool HadRecentDentalVisit { get; set; }
        public string? RecentVisitReason { get; set; }

        // الخلع
        public bool HadPreviousExtraction { get; set; }
        public bool HadAnestheticAllergy { get; set; }
        public bool HadDelayedHealing { get; set; }
        public bool HadBleeding { get; set; }
        public bool HadFainting { get; set; }

        // التركيبات
        public bool HadPreviousProsthetics { get; set; }
        public string? ProstheticsType { get; set; }

        // إشعاع أو كيماوي
        public bool HadRadiationOrChemo { get; set; }

        // للنساء فقط
        public bool IsPregnant { get; set; }
        public bool IsBreastfeeding { get; set; }

        public string? Notes { get; set; }
    }
}
