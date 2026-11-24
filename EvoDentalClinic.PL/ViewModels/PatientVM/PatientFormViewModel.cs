using System.ComponentModel.DataAnnotations;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.PL.ViewModels;

public class PatientFormViewModel
{
    public int Id { get; set; }

    // للعرض في شاشة التعديل بس، مش بيتبعت ولا بيتعدّل
    public string? Code { get; set; }

    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(150, ErrorMessage = "الاسم طويل جداً")]
    [Display(Name = "الاسم")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "رقم الموبايل مطلوب")]
    [StringLength(15, ErrorMessage = "رقم الموبايل طويل جداً")]
    [Display(Name = "رقم الموبايل")]
    public string Phone { get; set; } = null!;

    [Display(Name = "تاريخ الميلاد")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "النوع مطلوب")]
    [Display(Name = "النوع")]
    public Gender? Gender { get; set; }

    [Display(Name = "الحالة الاجتماعية")]
    public MaritalStatus? MaritalStatus { get; set; }

    [StringLength(250, ErrorMessage = "العنوان طويل جداً")]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "الوظيفة طويلة جداً")]
    [Display(Name = "الوظيفة")]
    public string? Job { get; set; }
}