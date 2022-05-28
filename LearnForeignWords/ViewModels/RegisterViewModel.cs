using System.ComponentModel.DataAnnotations;

namespace LearnForeignWords.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указан Email ")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Не указан Пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Не указан Пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
