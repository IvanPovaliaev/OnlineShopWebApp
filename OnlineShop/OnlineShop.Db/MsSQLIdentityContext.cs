using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class MsSQLIdentityContext : IdentityContext
    {
        public MsSQLIdentityContext(DbContextOptions<MsSQLIdentityContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
