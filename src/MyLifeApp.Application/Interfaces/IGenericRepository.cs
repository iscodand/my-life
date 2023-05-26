namespace MyLifeApp.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<ICollection<T>> GetAll();
        public Task<T> GetById(Guid id);
        public Task<T> Create(T entity);
        public Task Update(T entity);
        public Task Delete(T entity);
        public Task Save();
    }
}