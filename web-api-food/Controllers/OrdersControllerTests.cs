using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using wep_api_food.Controllers;
using wep_api_food.Dtos;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;
using wep_api_food.Services.Intefaces;

namespace web_api_food.Controllers
{
    public class OrdersControllerTests
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBusService<OrderMessage> _messageBusService;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrdersController> _logger;
        private readonly OrdersController _ordersController;


        public OrdersControllerTests()
        {
            //Dependecies
            _mapper = A.Fake<IMapper>();
            _orderRepository = A.Fake<IOrderRepository>();
            _messageBusService = A.Fake<IMessageBusService<OrderMessage>>();
            _productRepository = A.Fake<IProductRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _logger = A.Fake<ILogger<OrdersController>>();
            //SUT
            _ordersController = new OrdersController(_mapper,_orderRepository,_messageBusService,_productRepository,_userRepository,_logger);
        }
        [Fact]
        public async Task CreateOrder_Success()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _ordersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var orderDto = new OrderCreateModel
            {
                Address = "123 Main St",
                ProductsInOrder = new List<ProductsInOrder>()
            {
                new ProductsInOrder(){Quantity = 10, Product = new ProductReadModel{
                } }
            }
            };

            A.CallTo(() => _productRepository.Get(A<Guid>._)).Returns(Task.FromResult(new Product { Quantity = 100 }));
            A.CallTo(() => _orderRepository.Create(A<Order>._, A<List<OrderProduct>>._)).Returns(true);
            // Act
            var result = await _ordersController.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }
        [Fact]
        public async Task CreateOrder_NullDto_ReturnBadRequest()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _ordersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            OrderCreateModel orderDto = null;
            // Act
            var result = await _ordersController.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task CreateOrder_NotEnoughProductEx()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _ordersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var orderDto = new OrderCreateModel { Address = "123 Main St", ProductsInOrder = new List<ProductsInOrder>() 
            { 
                new ProductsInOrder(){Quantity = 10, Product = new ProductReadModel{ } }
            }
            };

            A.CallTo(() => _productRepository.Get(A<Guid>._)).Returns(Task.FromResult(new Product { Quantity = 0 }));

            // Act
            var result = await _ordersController.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(501);

        }
        [Fact]
        public async Task CreateOrder_NotFoundProductEx()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _ordersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var orderDto = new OrderCreateModel
            {
                Address = "123 Main St",
                ProductsInOrder = new List<ProductsInOrder>()
            {
                new ProductsInOrder(){Quantity = 10, Product = new ProductReadModel{ } }
            }
            };
            A.CallTo(() => _productRepository.Get(A<Guid>._)).Returns(Task.FromResult<Product>(null));
            //Act
            var result = await _ordersController.CreateOrder(orderDto);
            //Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(502);

        }
    }
}
