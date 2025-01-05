using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Infrastructure.Data
{
    public class PostgreSQLContext : DatabaseContext
    {
        public PostgreSQLContext(DbContextOptions<PostgreSQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
