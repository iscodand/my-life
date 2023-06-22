namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetByIdAsync(string id);
        public Task<T> CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
        public Task SaveAsync();
    }
}