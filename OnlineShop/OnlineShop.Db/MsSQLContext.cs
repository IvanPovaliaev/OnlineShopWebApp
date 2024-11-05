using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Db
{
    public class MsSQLContext : DatabaseContext
    {
        public MsSQLContext(DbContextOptions<MsSQLContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
