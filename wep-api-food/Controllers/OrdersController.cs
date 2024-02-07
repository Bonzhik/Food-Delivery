using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using wep_api_food.Dtos;
using wep_api_food.Enums;
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

        public OrdersController(IMapper mapper,
            IOrderRepository orderRepository,
            IMessageBusService<OrderMessage> messageBusService,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _messageBusService = messageBusService;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateOrder(OrderCreateModel orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest();
            }
            Order order = new Order();
            order.Id = Guid.NewGuid();
            var user = await _userRepository.Get(User.FindFirstValue(ClaimTypes.NameIdentifier));
            order.User = user;
            order.Status = Enums.OrderStatuses.Create;
            List<OrderProduct> orderProductsEntities = new List<OrderProduct>();
            foreach (var product in orderDto.ProductsInOrder)
            {
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
        public async Task<IActionResult> OrderHook(OrderNotify orderNotify)
        {
            var order = await _orderRepository.Get(orderNotify.Id);
            order.Status = orderNotify.Status;
            await _orderRepository.Save();

            return Ok(order);
        }
    }
}
