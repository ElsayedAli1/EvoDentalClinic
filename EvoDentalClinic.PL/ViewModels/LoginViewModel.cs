using System.ComponentModel.DataAnnotations;

namespace EvoDentalClinic.PL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "الإيميل أو رقم الموبايل مطلوب")]
        [Display(Name = "الإيميل أو رقم الموبايل")]
        public string Identifier { get; set; } = null!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = null!;

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}
