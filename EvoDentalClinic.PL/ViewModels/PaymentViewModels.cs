using System.ComponentModel.DataAnnotations;
using EvoDentalClinic.BLL.DTOs.Payments;
using EvoDentalClinic.BLL.DTOs.Treatments;

namespace EvoDentalClinic.PL.ViewModels;

// اللي بيتبعت من الفورم
public class PaymentInputModel
{
    [Required(ErrorMessage = "المبلغ مطلوب")]
    [Display(Name = "المبلغ")]
    public decimal? Amount { get; set; }

    [StringLength(250, ErrorMessage = "الملاحظة طويلة جداً")]
    [Display(Name = "ملاحظة")]
    public string? Note { get; set; }
}

// اللي بيتعرض في الصفحة (مش بيتبعت)
public class PaymentPageViewModel
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public TreatmentRecordDto Treatment { get; set; } = null!;
    public List<PaymentDto> History { get; set; } = new();
    public PaymentInputModel Input { get; set; } = new();
}