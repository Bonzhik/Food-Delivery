using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using wep_api_food.Dtos;
using wep_api_food.Enums;
using wep_api_food.Exceptions;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;
using wep_api_food.Services.Intefaces;

namespace wep_api_food.Controllers
{
    [ApiController]
    [Route("api/pub/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBusService<OrderMessage> _messageBusService;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMapper mapper,
            IOrderRepository orderRepository,
            IMessageBusService<OrderMessage> messageBusService,
            IProductRepository productRepository,
            IUserRepository userRepository,
            ILogger<OrdersController> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _messageBusService = messageBusService;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateOrder(OrderCreateModel orderDto)
        {
            if (orderDto == null)
            {
                _logger.LogWarning($"Пришел пустой заказ {DateTime.UtcNow}");
                return BadRequest();
            }
            Order order = new Order();
            order.Id = Guid.NewGuid();
            var user = await _userRepository.Get(User.FindFirstValue(ClaimTypes.NameIdentifier));
            order.User = user;
            order.Status = Enums.OrderStatuses.Create;
            order.Address = orderDto.Address;
            List<OrderProduct> orderProductsEntities = new List<OrderProduct>();
            foreach (var product in orderDto.ProductsInOrder)
            {
                try
                {
                    var productCheck = await _productRepository.Get(product.Product.Id);
                    if (productCheck == null)
                    {
                        throw new NotFoundException($"Не найден продукт {product.Product.Id}");
                    }
                    if (productCheck.Quantity < product.Quantity)
                    {
                        throw new NotEnoughProductsException($"Недостаточно продуктов {productCheck.Id} для заказа {order.Id}");
                    }
                }
                catch(NotEnoughProductsException ex)
                {
                    _logger.LogWarning(ex.Message, DateTime.UtcNow);
                    return StatusCode(501, ex.Message);
                }
                catch(NotFoundException ex)
                {
                    _logger.LogWarning(ex.Message, DateTime.UtcNow);
                    return StatusCode(502, ex.Message);
                }
                var orderProduct = new OrderProduct()
                {
                    Order = order,
                    Product = await _productRepository.Get(product.Product.Id),
                    Quantity = product.Quantity
                };
                orderProductsEntities.Add(orderProduct);
            }

            if (!await _orderRepository.Create(order, orderProductsEntities))
            {
                return StatusCode(500, "Internal Server Error");
            }

            var orderMessage = new OrderMessage()
            {
                Id = order.Id,
                ProductsInOrder = orderDto.ProductsInOrder,
                Status = OrderStatuses.Create,
                Address = orderDto.Address
            };

            _messageBusService.SendMessage(orderMessage);

            return Ok(orderDto);
        }

        [HttpPost]
        [Route("Notify")]
        public async Task<IActionResult> OrderHook(OrderNotify orderNotify)
        {
            var order = await _orderRepository.Get(orderNotify.Id);
            order.Status = orderNotify.Status;
            await _orderRepository.Save(order);

            return Ok();
        }
    }
}
