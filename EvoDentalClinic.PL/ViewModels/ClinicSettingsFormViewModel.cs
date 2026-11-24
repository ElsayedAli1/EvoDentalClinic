using System.ComponentModel.DataAnnotations;

namespace EvoDentalClinic.PL.ViewModels;

public class ClinicSettingsFormViewModel
{
    public List<DayOfWeek> WorkDays { get; set; } = new();

    [Required(ErrorMessage = "بداية الشغل مطلوبة")]
    [Display(Name = "بداية الشغل")]
    public TimeOnly? OpenTime { get; set; }

    [Required(ErrorMessage = "نهاية الشغل مطلوبة")]
    [Display(Name = "نهاية الشغل")]
    public TimeOnly? CloseTime { get; set; }

    [Required(ErrorMessage = "مدة الكشف مطلوبة")]
    [Display(Name = "مدة الكشف")]
    public int? SlotMinutes { get; set; }
}