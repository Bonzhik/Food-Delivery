using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Enums;
using wep_api_food_delivery.Factories;
using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Services.Implementations
{
    public class EventProcessor
    {
        private readonly IDbContextFactory _contextFactory;

        public EventProcessor(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task HandleEvent(OrderMessage orderMessage)
        {
            switch (orderMessage.Status)
            {
                case OrderStatuses.Create:
                    await OnCreate(orderMessage);
                    break;
                case OrderStatuses.Cancel:
                    await OnCansel(orderMessage);
                    break;
            }
        }
        private async Task<bool> OnCreate(OrderMessage orderMessage)
        {
            try
            {
                using (var _context = _contextFactory.CreateDbContext())
                {
                    var order = new Order()
                    {
                        Id = orderMessage.Id,
                        Address = orderMessage.Address,
                        Status = orderMessage.Status,
                        User = null
                    };
                    order.OrderItems = orderMessage.ProductsInOrder.Select(item => new OrderItem { Id = Guid.NewGuid(), Quantity = item.Quantity, Title = item.Product.Title, Order = order }).ToList();
                    await _context.AddAsync(order);
                    return await _context.SaveChangesAsync() > 0 ? true : false;
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Failed to CreateEvent ---> {ex.Message}");
                return false;
            }
            
        }
        private async Task<bool> OnCansel(OrderMessage orderMessage)
        {
            try
            {
                using (var _context = _contextFactory.CreateDbContext())
                {
                    var order = await _context.Orders.FindAsync(orderMessage.Id);
                    _context.Remove(order);
                    return await _context.SaveChangesAsync() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to CanselEvent ---> {ex.Message}");
                return false;
            }
        }
    }
}
