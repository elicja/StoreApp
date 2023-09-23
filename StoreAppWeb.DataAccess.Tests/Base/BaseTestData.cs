using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoreApp.DataAccess.Data;

namespace StoreAppWeb.DataAccess.Tests.Base
{
    public class BaseTestData
    {
        protected AppDbContext _context;

        public BaseTestData()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider).Options;

            _context = new AppDbContext(opts);
        }
    }
}
