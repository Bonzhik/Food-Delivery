using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wep_api_food.Services.Implementations;
using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Services.Intefaces;

namespace wep_api_food_delivery.Controllers
{
    [ApiController]
    [Route("api/sub/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthDataClient _authDataClient;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, IAuthDataClient authDataClient, ILogger<UsersController> logger)
        {
            _context = context;
            _authDataClient = authDataClient;
            _logger = logger;
        }

        [HttpPost] 
        public async Task<IActionResult> Login (UserLoginModel userDto)
        {
            var user =await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email && u.Password == userDto.Password);
            if (user == null)
            {
                _logger.LogWarning($"Не удалось войти в аккаунт {userDto.Email}");
                return NotFound();
            }
            var token = await _authDataClient.ReturnTokenAsync(user.Email, user.Role.ToString());

            var userRead = new UserReadModel()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                Password = userDto.Password
            };
            return Ok(userRead);
        }
    }
}
