using OnlineShop.Application.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [PasswordValidation()]
        public string Password { get; init; }

        [Required(ErrorMessage = "Введите пароль повторно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Не указан токен")]
        public string? Token { get; init; }
    }
}
