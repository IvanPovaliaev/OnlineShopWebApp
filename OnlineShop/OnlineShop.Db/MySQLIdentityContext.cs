using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class MySQLIdentityContext : IdentityContext
    {
        public MySQLIdentityContext(DbContextOptions<MySQLIdentityContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
