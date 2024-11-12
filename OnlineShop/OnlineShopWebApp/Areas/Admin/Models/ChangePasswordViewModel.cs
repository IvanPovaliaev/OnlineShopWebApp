using OnlineShopWebApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [PasswordValidation()]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
