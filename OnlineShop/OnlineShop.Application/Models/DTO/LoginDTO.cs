using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.DTO
{
	public class LoginDTO
	{
		[Required(ErrorMessage = "Не указан Email")]
		[EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
		public string Email { get; init; }

		[Required(ErrorMessage = "Введите пароль")]
		public string Password { get; init; }
	}
}
