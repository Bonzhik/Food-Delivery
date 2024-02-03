using wep_api_food.Enums;
using wep_api_food.Models;

namespace wep_api_food.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<bool> IsExists(string title);
    }
}
