using Grpc.Net.Client;
using web_api_food_delivery;
using wep_api_food_delivery.Services.Intefaces;

namespace wep_api_food.Services.Implementations
{
    public class AuthDataClient : IAuthDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthDataClient> _logger;

        public AuthDataClient(IConfiguration configuration, ILogger<AuthDataClient> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> ReturnTokenAsync(string email, string role)
        {
            try
            {
                var channel = GrpcChannel.ForAddress(_configuration["GrpcAuth"]);
                var client = new GrpcToken.GrpcTokenClient(channel);
                var request = new CreateTokenRequest()
                {
                    Email = email,
                    Role = role,
                    Audience = _configuration["Audience"],
                    Key = _configuration["SecretKey"]
                };
                try
                {
                    var reply = await client.CreateTokenAsync(request);
                    return reply.Token;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Ошибка при вызове gRPC-сервера для пользователя {@email} с ролью {@role} {@DateTimeNow},",
                        email,
                        role,
                        DateTime.UtcNow);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при создании канала gRPC для адреса {_configuration["GrpcAuth"]}, {DateTime.UtcNow}");
                return null;
            }
        }
    }
}
