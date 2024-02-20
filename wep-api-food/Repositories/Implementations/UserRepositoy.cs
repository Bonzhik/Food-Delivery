using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using wep_api_food.Data;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;

namespace wep_api_food.Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger) : base(context, logger)
        {
        }

        public async Task<User> Get(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<bool> IsExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
