using wep_api_food_delivery.Data;

namespace wep_api_food_delivery.Factories
{
    public interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }
}
