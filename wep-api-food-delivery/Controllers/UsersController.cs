using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wep_api_food.Services.Implementations;
using wep_api_food_delivery.Data;
using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Services.Intefaces;

namespace wep_api_food_delivery.Controllers
{
    [ApiController]
    [Route("api/sub/[conroller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthDataClient _authDataClient;

        public UsersController(ApplicationDbContext context, IAuthDataClient authDataClient)
        {
            _context = context;
            _authDataClient = authDataClient;
        }

        [HttpGet] 
        public async Task<IActionResult> Login (UserLoginModel userDto)
        {
            var user =await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email && u.Password == userDto.Password);
            if (user == null)
            {
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
