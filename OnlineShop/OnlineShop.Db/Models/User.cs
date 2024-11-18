using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Db.Models
{
	public class User : IdentityUser
	{
		public string? FullName { get; set; }
		public string? AvatarUrl { get; set; }
	}
}
