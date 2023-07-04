using System.Text.Json;
using MyLifeApp.Application.Interfaces.Services;
using StackExchange.Redis;

namespace MyLifeApp.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;

        public CacheService()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis:6379");
            _cacheDb = redis.GetDatabase();
        }

        public async Task<T> GetData<T>(string key)
        {
            RedisValue value = await _cacheDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expirityTime = expirationTime.DateTime.Subtract(DateTime.Now); 

            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expirityTime);        
        }

        public async Task<object> RemoveData(string key)
        {
            bool keyExists = await _cacheDb.KeyExistsAsync(key);

            if (keyExists)
            {
                return await _cacheDb.KeyDeleteAsync(key);
            }

            return false;
        }
    }
}