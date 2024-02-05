using Microsoft.EntityFrameworkCore;
using wep_api_food.Data;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;

namespace wep_api_food.Repositories.Implementations
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly BaseRepository<Order> _baseRepository;

        public OrderRepository(ApplicationDbContext context,
            BaseRepository<Order> baseRepository) : base(context) 
        {
            _baseRepository = baseRepository;
        }

        public async Task<bool> Create(Order order, List<OrderProduct> orderProducts)
        {
            await _baseRepository.Create(order);

            foreach (var product in orderProducts)
            {
                await _context.AddAsync(product);
            }
            return await Save();
        }
        public async Task<bool> Update(Order order, List<OrderProduct> orderProducts)
        {
            var onDeleteEntities = _context.OrderProducts.
                Where(p => p.OrderId == order.Id).ToList();
            _context.RemoveRange(onDeleteEntities);
            await _context.SaveChangesAsync();

            foreach (var product in orderProducts)
            {
                await _context.AddAsync(product);
            }
            return await Save();
        }
    }
}
