using System.ComponentModel.DataAnnotations;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.PL.ViewModels;

public class ReviewFormViewModel
{
    [Required(ErrorMessage = "اختار تقييم من 1 لـ 5 نجوم")]
    [Range(1, 5, ErrorMessage = "اختار تقييم من 1 لـ 5 نجوم")]
    [Display(Name = "التقييم")]
    public int? Rating { get; set; }

    [Required(ErrorMessage = "اكتب رأيك")]
    [StringLength(600, MinimumLength = 10, ErrorMessage = "الرأي لازم يكون بين 10 و600 حرف")]
    [Display(Name = "رأيك")]
    public string Comment { get; set; } = "";

    // للعرض بس، بتتجاب من الداتا بيز مش من الفورم
    public bool HasReview { get; set; }
    public ReviewStatus? CurrentStatus { get; set; }
}