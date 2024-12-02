using OnlineShop.Application.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Abstractions
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Не указан Email")]
		[EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Введите пароль")]
		[PasswordValidation()]
		public string Password { get; set; }

		[Required(ErrorMessage = "Введите пароль")]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public string ConfirmPassword { get; set; }
		public string? Name { get; set; }

		[PhoneValidation()]
		public string? PhoneNumber { get; set; }
	}
}
