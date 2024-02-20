using System.Text;
using System.Text.Json;
using wep_api_food_delivery.Dtos;

namespace wep_api_food_delivery.Services.Implementations
{
    public class HttpClientSender
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HttpClientSender> _logger;

        public HttpClientSender(HttpClient client, IConfiguration configuration, ILogger<HttpClientSender> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;

        }

        public async Task NotifyAboutOrder(OrderNotify order)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(_configuration["FoodNotify"],httpContent);

            if (response.IsSuccessStatusCode)
            {
               _logger.LogInformation($"--> Sync POST to FoodService was OK! {DateTime.UtcNow}");
            }
            else
            {
               _logger.LogInformation($"--> Sync POST to FoodService was not OK! {DateTime.UtcNow}");
            }
        }
    }
}
