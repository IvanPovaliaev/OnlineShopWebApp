using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Infrastructure.Data
{
    public class MsSQLContext : DatabaseContext
    {
        public MsSQLContext(DbContextOptions<MsSQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
