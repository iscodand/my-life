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

        public async Task<string> GetDataAsync(string key)
        {
            RedisValue value = await _cacheDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return default;
        }

        public async Task<bool> SetDataAsync(string key, object value, DateTimeOffset expirationTime)
        {
            TimeSpan expirityTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expirityTime);
        }

        public async Task<object> RemoveDataAsync(string key)
        {
            bool keyExists = await _cacheDb.KeyExistsAsync(key);

            if (keyExists)
            {
                return await _cacheDb.KeyDeleteAsync(key);
            }

            return false;
        }

        public async Task RefreshCacheAsync(string key, object value, DateTimeOffset expirationTime)
        {
            await this.RemoveDataAsync(key);
            await this.SetDataAsync(key, JsonSerializer.Serialize(value), expirationTime);
        }
    }
}