using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Services.Implementations;

namespace wep_api_food_delivery.Controllers
{
    [ApiController]
    [Route("api/sub/[conroller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClientSender _httpClient;

        public OrdersController(ApplicationDbContext context, HttpClientSender httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {

            var ordersRead = new List<OrderReadModel>();
            var orders = await _context.Orders.Include(o=>o.OrderItems).ToListAsync();

            foreach(var order  in orders)
            {
                ordersRead.Add(new OrderReadModel()
                {
                    Id = order.Id,
                    Address = order.Address,
                    Status = order.Status.ToString(),
                    Items = order.OrderItems.Select(item => new OrderReadItemModel { Quantity = item.Quantity, Title = item.Title }).ToList(),
            });
            }
            return Ok(ordersRead);
        }
    }
}
