using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Domain.Models
{
    public class Role : IdentityRole
    {
        public bool CanBeDeleted { get; init; } = true;
    }
}
