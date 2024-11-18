using OnlineShopWebApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
	public class EditUserViewModel
	{
		[Required(ErrorMessage = "Не указан UserId")]
		public required string Id { get; init; }

		[Required(ErrorMessage = "Не указан Email")]
		[EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
		public string Email { get; set; }
		public string? FullName { get; set; }

		[PhoneValidation()]
		public string? PhoneNumber { get; set; }
	}
}
