using System.ComponentModel.DataAnnotations;

namespace EvoDentalClinic.PL.ViewModels;

public class TreatmentFormViewModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string? PatientName { get; set; }

    [Required(ErrorMessage = "التاريخ مطلوب")]
    [Display(Name = "تاريخ الإجراء")]
    public DateOnly? Date { get; set; }

    [Required(ErrorMessage = "رقم السن مطلوب")]
    [Display(Name = "رقم السن")]
    public int? ToothNumber { get; set; }

    [Required(ErrorMessage = "اسم الإجراء مطلوب")]
    [StringLength(200, ErrorMessage = "اسم الإجراء طويل جداً")]
    [Display(Name = "الإجراء")]
    public string Procedure { get; set; } = null!;

    [Required(ErrorMessage = "التكلفة مطلوبة")]
    [Display(Name = "التكلفة الكلية")]
    public decimal? TotalCost { get; set; }
}