using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class PostgreSQLIdentityContext : IdentityContext
    {
        public PostgreSQLIdentityContext(DbContextOptions<PostgreSQLIdentityContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
