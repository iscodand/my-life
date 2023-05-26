using MyLifeApp.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Interfaces;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await Save();
            return entity;
        }

        public async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await Save();
        }

        public async Task Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            await Save();
        }

        public Task Save()
        {
            return _context.SaveChangesAsync();
        }
    }
}