using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; init; }

        public bool KeepMeLogged { get; init; }

        public string ReturnUrl { get; set; }
    }
}
