namespace MyLifeApp.Application.Interfaces.Services
{
    public interface ICacheService
    {
        public Task<T> GetData<T>(string key);
        public Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime);
        public Task<object> RemoveData(string key);
    }
}