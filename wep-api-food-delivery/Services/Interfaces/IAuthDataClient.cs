namespace wep_api_food_delivery.Services.Intefaces
{
    public interface IAuthDataClient
    {
        Task<string> ReturnTokenAsync(string email, string role);
    }
}
