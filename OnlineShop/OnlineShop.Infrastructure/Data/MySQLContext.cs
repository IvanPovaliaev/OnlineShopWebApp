using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Infrastructure.Data
{
    public class MySQLContext : DatabaseContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
