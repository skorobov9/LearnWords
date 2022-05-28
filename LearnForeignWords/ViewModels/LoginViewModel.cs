using System.ComponentModel.DataAnnotations;

namespace LearnForeignWords.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указан Email ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Не указан Пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
