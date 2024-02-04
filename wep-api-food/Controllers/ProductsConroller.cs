using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wep_api_food.Dtos;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;
using wep_api_food.Services.Implementations;
using wep_api_food.Services.Intefaces;

namespace wep_api_food.Controllers
{
    [ApiController]
    [Route("api/pub/[controller]")]
    public class ProductsConroller : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public ProductsConroller(
            IProductRepository productRepository, 
            ICacheService cacheService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetProducts()
        {
            var cacheData = await _cacheService.GetDataAsync<ICollection<Product>>("products");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }
            var products = await _productRepository.GetAll();
            var expTime = DateTime.Now.AddMinutes(10);
            await _cacheService.SetDataAsync("products", products, expTime);

            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = _productRepository.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ProductCreateModel productDto)
        {
            if (productDto == null)
            {
                return BadRequest();
            }

            if (await _productRepository.IsExists(productDto.Title))
            {
                return Conflict();
            }

            var product = _mapper.Map<Product>(productDto);
            product.Id = Guid.NewGuid();

            if (! await _productRepository.Create(product)) {
                return StatusCode(500, "Internal Server Error");
            }

            var cacheData = await _cacheService.GetDataAsync<ICollection<Product>>("products");
            var expTime = DateTime.Now.AddMinutes(10);
            if (cacheData != null)
            {
                cacheData.Add(product);
                await _cacheService.SetDataAsync("products", cacheData, expTime);
            }
            else
            {
                var products = await _productRepository.GetAll();
                await _cacheService.SetDataAsync("products", products, expTime);
            }

            return Ok(product);
        }
    }
}
