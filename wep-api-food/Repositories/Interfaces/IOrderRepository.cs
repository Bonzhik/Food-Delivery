using wep_api_food.Models;

namespace wep_api_food.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<bool> Create(Order order, Dictionary<Guid, int> products);
        Task<bool> Update(Order order, Dictionary<Guid, int> products);
    }
}
