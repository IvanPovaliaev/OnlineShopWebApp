using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Models;

namespace OnlineShop.Db
{
    public abstract class IdentityContext : IdentityDbContext<User, Role, string>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }
    }
}
