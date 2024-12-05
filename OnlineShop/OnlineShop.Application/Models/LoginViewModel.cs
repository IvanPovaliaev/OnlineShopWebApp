using OnlineShop.Application.Models.DTO;

namespace OnlineShop.Application.Models
{
	public class LoginViewModel : LoginDTO
	{
		public bool KeepMeLogged { get; init; }

		public string ReturnUrl { get; set; }
	}
}
