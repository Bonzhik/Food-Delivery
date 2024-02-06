using System.Text;
using System.Text.Json;
using wep_api_food_delivery.Dtos;

namespace wep_api_food_delivery.Services.Implementations
{
    public class HttpClientSender
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public HttpClientSender(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task NotifyAboutOrder(OrderNotify order)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(_configuration["Food"],httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to FoodService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to FoodService was NOT OK!");
            }
        }
    }
}
