using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Domain.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
