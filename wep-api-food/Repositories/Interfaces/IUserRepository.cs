using wep_api_food.Models;

namespace wep_api_food.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> Get(string email);
    }
}
