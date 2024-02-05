using wep_api_food.Models;

namespace wep_api_food.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<bool> Create(Order order, List<OrderProduct> orderProducts);
        Task<bool> Update(Order order, List<OrderProduct> orderProducts);
    }
}
