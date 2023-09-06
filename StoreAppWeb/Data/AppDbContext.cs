using Microsoft.EntityFrameworkCore;

namespace StoreAppWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
        {
            
        }
    }
}
