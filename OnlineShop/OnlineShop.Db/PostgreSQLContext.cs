using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class PostgreSQLContext : DatabaseContext
    {
        public PostgreSQLContext(DbContextOptions<PostgreSQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
