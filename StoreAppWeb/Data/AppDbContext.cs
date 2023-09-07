using Microsoft.EntityFrameworkCore;
using StoreAppWeb.Models;

namespace StoreAppWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
    }
}