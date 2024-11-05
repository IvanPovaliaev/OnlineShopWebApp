using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class MySQLContext : DatabaseContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
