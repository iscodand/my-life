using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Application.Interfaces;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class RefactorProfileRepository : GenericRepository<Profile>, IRefactorProfileRepository
    {
        public RefactorProfileRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
