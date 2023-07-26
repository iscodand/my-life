using MyLifeApp.Infrastructure.Identity.Models;

namespace MyLifeApp.Infrastructure.Identity.Interfaces.Services
{
    public interface IAuthenticatedUserService
    {
        public Task<User> GetAuthenticatedUserAsync();
    }
}