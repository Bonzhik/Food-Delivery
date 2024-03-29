﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        private readonly ILogger<ProductsConroller> _logger;

        public ProductsConroller(
            IProductRepository productRepository, 
            ICacheService cacheService,
            IMapper mapper,
            ILogger<ProductsConroller> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;

        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var cacheData = await _cacheService.GetDataAsync<ICollection<Product>>("products");
            if (cacheData != null)
            {
                var returnProduct = _mapper.Map<ICollection<ProductReadModel>>(cacheData);
                return Ok(returnProduct);
            }
            var products = _mapper.Map<ICollection<ProductReadModel>>(await _productRepository.GetAll());
            var expTime = DateTime.Now.AddMinutes(10);
            await _cacheService.SetDataAsync("products", products, expTime);

            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = _mapper.Map<ProductReadModel>(_productRepository.Get(id));
            if (product == null)
            {
                _logger.LogWarning($"Попытка получить несуществующий продукт {id} - {DateTime.UtcNow}");
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
                _logger.LogWarning($"Получена пустая форма на создание продукта {DateTime.UtcNow}");
                return BadRequest();
            }

            if (await _productRepository.IsExists(productDto.Title))
            {
                _logger.LogWarning($"Попытка добавить продукт с уже существующим именем {productDto.Title} - {DateTime.UtcNow}");
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
