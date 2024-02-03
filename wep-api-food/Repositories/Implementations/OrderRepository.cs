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

        public async Task<bool> Create(Order order, Dictionary<Guid, int> products)
        {
            await _baseRepository.Create(order);

            foreach (var product in products)
            {
                var orderProductEntity = new OrderProduct()
                {
                    Product = await _context.Products.FindAsync(product.Key),
                    Order = order,
                    Quantity = product.Value
                };
                await _context.AddAsync(orderProductEntity);
            }
            return await Save();
        }
        public async Task<bool> Update(Order order, Dictionary<Guid, int> products)
        {
            var onDeleteEntities = _context.OrderProducts.
                Where(p => p.OrderId == order.Id).ToList();
            _context.RemoveRange(onDeleteEntities);
            await _context.SaveChangesAsync();

            foreach (var product in products)
            {
                var orderProductEntity = new OrderProduct()
                {
                    Product = await _context.Products.FindAsync(product.Key),
                    Order = order,
                    Quantity = product.Value
                };
                _context.Add(orderProductEntity);
            }
            return await Save();
        }
    }
}
