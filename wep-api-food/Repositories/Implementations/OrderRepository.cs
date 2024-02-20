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
            BaseRepository<Order> baseRepository, ILogger<OrderRepository> logger) : base(context, logger) 
        {
            _baseRepository = baseRepository;
        }
        public async Task<bool> Create(Order order, List<OrderProduct> orderProducts)
        {
            await _baseRepository.Create(order);
            foreach (var product in orderProducts)
            {
                _logger.LogInformation(
                    "Попытка добавить в заказ {@OrderId}, продукт {@ProductId}",
                    order.Id,
                    product.ProductId);
                try
                {
                    await _context.AddAsync(product);
                }catch (Exception ex)
                {
                    _logger.LogError(
                        "Ошибка добавить в заказ {@OrderId}, продукт {@ProductId}",
                        order.Id,
                        product.ProductId);
                }
            }
            return await Save(order);
        }
        public async Task<bool> Update(Order order, List<OrderProduct> orderProducts)
        {
            _logger.LogInformation(
                    "Попытка обновить заказ {@OrderId}",
                    order.Id);
            try
            {
                var onDeleteEntities = _context.OrderProducts.
                Where(p => p.OrderId == order.Id).ToList();
                _context.RemoveRange(onDeleteEntities);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                _logger.LogError(
                        "Ошибка при обновлении связей у заказа {@OrderId}",
                        order.Id);
            }

            foreach (var product in orderProducts)
            {
                try
                {
                    await _context.AddAsync(product);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Ошибка добавить в заказ {@OrderId}, продукт {@ProductId}",
                        order.Id,
                        product.ProductId);
                }
            }
            return await Save(order);
        }
    }
}
