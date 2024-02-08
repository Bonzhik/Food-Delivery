using Microsoft.EntityFrameworkCore;
using wep_api_food_delivery.Data;

namespace wep_api_food_delivery.Factories
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }
        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
