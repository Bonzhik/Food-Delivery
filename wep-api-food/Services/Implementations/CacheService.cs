using StackExchange.Redis;
using System.Text.Json;
using wep_api_food.Services.Intefaces;

namespace wep_api_food.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private IDatabase _cacheDb;
        public CacheService(IConfiguration configuration)
        {
            _configuration = configuration;
            try
            {
                var redis = ConnectionMultiplexer.Connect(_configuration["Redis"]);
                _cacheDb = redis.GetDatabase();
            } catch (Exception ex)
            {

            }
        }
        public async Task<bool> DeleteDataAsync<T>(string key)
        {
            if (await _cacheDb.KeyExistsAsync(key))
            {
                return await _cacheDb.KeyDeleteAsync(key);
            }
            return false;
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Данные из кеша");
                return JsonSerializer.Deserialize<T>(value);
            }
            return default(T);
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expTime);
        }
    }
}
