using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; init; }
    }
}
