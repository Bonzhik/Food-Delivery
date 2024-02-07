using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Enums;
using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Services.Implementations
{
    public class EventProcessor
    {
        private readonly ApplicationDbContext _context;

        public EventProcessor(ApplicationDbContext context)
        {
            _context = context;
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
        private async Task<bool> OnCansel(OrderMessage orderMessage)
        {
            var order = await _context.Orders.FindAsync(orderMessage.Id);
            _context.Remove(order);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
