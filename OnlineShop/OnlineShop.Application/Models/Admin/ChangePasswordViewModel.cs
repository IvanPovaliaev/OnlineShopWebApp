using OnlineShop.Application.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Admin
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; init; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [PasswordValidation()]
        public string Password { get; init; }

        [Required(ErrorMessage = "Повторите новый пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; init; }
    }
}
