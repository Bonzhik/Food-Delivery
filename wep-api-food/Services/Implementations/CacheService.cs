using StackExchange.Redis;
using System.Text.Json;
using wep_api_food.Services.Intefaces;

namespace wep_api_food.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private IDatabase _cacheDb;
        private readonly ILogger<CacheService> _logger;
        public CacheService(IConfiguration configuration, ILogger<CacheService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            try
            {
                var redis = ConnectionMultiplexer.Connect(_configuration["Redis"]);
                _cacheDb = redis.GetDatabase();
            } catch (Exception ex)
            {
                _logger.LogError($"Ошибка подключения к CacheService {DateTime.UtcNow}");
            }
        }
        public async Task<bool> DeleteDataAsync<T>(string key)
        {
            try
            {
                if (await _cacheDb.KeyExistsAsync(key))
                {
                    return await _cacheDb.KeyDeleteAsync(key);
                }
                return false;
            }catch (Exception ex)
            {
                _logger.LogError($"Ошибка при удалении ключа {key} - {DateTime.UtcNow}");
                return false;
            }
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            try
            {
                var value = await _cacheDb.StringGetAsync(key);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonSerializer.Deserialize<T>(value);
                }
                return default(T);
            } catch (Exception ex)
            {
                _logger.LogError($"Ошибка при получении данных по ключу {key} - {DateTime.UtcNow}");
                return default(T);
            }
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            try
            {
                var expTime = expirationTime.DateTime.Subtract(DateTime.Now);
                return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expTime);
            }catch (Exception ex)
            {
                _logger.LogError($"Ошибка при установке данных по ключу {key} - {DateTime.UtcNow}");
                return false;
            }
            
        }
    }
}
