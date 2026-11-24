using System.ComponentModel.DataAnnotations;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.PL.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(150, ErrorMessage = "الاسم طويل جداً")]
    [Display(Name = "الاسم")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "رقم الموبايل مطلوب")]
    [StringLength(20, ErrorMessage = "رقم الموبايل طويل جداً")]
    [Display(Name = "رقم الموبايل")]
    public string Phone { get; set; } = null!;

    [EmailAddress(ErrorMessage = "الإيميل غير صحيح")]
    [Display(Name = "الإيميل (اختياري)")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "النوع مطلوب")]
    [Display(Name = "النوع")]
    public Gender? Gender { get; set; }

    [Display(Name = "تاريخ الميلاد (اختياري)")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "كلمة المرور لازم تكون 8 حروف على الأقل")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "كلمتا المرور غير متطابقتين")]
    [Display(Name = "تأكيد كلمة المرور")]
    public string ConfirmPassword { get; set; } = null!;
}