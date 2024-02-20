using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Services.Implementations;

namespace wep_api_food_delivery.Controllers
{
    [ApiController]
    [Route("api/sub/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClientSender _httpClient;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationDbContext context, HttpClientSender httpClient, ILogger<OrdersController> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
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

        [HttpPost]
        [Authorize(Roles = "Courier")]
        [Route("take/{id}")]
        public async Task<IActionResult> TakeOrder(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                var order = await _context.Orders.FindAsync(id);
                order.User = user;
                order.Status = Enums.OrderStatuses.Delivery;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, $"Ошибка сохранения заказа в базу {DateTime.UtcNow}");
                    return StatusCode(500, $"Ошибка сохранения заказа в базу {DateTime.UtcNow}");
                }
                await _httpClient.NotifyAboutOrder(new OrderNotify { Id = id, Status = order.Status });
            } catch (Exception ex)
            {
                _logger.LogError($"Во время принятия заказа произошла ошибка -> {ex.Message}");
                return StatusCode(503, $"Ошибка принятия заказа {DateTime.UtcNow}");
            }

            return Ok("Courier take order");
        }

        [HttpPost]
        [Authorize(Roles = "Courier")]
        [Route("cansel/{id}")]
        public async Task<IActionResult> CanselOrderDelivery(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                var order = await _context.Orders.FindAsync(id);
                order.User = null;
                order.Status = Enums.OrderStatuses.Create;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, $"Ошибка сохранения заказа в базу {DateTime.UtcNow}");
                    return StatusCode(500, $"Ошибка сохранения заказа в базу {DateTime.UtcNow}");
                }

                await _httpClient.NotifyAboutOrder(new OrderNotify { Id = id, Status = order.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во время отмены заказа произошла ошибка -> {ex.Message}");
                return StatusCode(503, $"Ошибка отмены заказа {DateTime.UtcNow}");
            }
            return Ok("Courier cansel order delivery");
        }
    }
}
