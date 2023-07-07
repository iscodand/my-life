namespace MyLifeApp.Application.Interfaces.Services
{
    public interface ICacheService
    {
        public Task<string> GetDataAsync(string key);
        public Task<bool> SetDataAsync(string key, object value, DateTimeOffset expirationTime);
        public Task<object> RemoveDataAsync(string key);
        public Task RefreshCacheAsync(string key, object value, DateTimeOffset expirationTime);
    }
}