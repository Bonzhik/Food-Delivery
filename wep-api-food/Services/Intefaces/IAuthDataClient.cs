using web_api_food;

namespace wep_api_food.Services.Intefaces
{
    public interface IAuthDataClient
    {
        Task<string> ReturnTokenAsync(string email, string role);
    }
}
