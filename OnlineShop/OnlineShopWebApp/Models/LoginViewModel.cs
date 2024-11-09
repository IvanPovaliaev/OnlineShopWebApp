using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; init; }

        public bool KeepMeLogged { get; init; }

        public string ReturnUrl { get; set; }
    }
}
