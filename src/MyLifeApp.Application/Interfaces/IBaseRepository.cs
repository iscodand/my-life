using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MyLifeApp.Application.Interfaces
{

    // Verify implementation of a Base Interface for Repositories
    public interface IBaseRepository
    {
        public Task<User> GetAuthenticatedUser(IHttpContextAccessor httpContext, UserManager<User> userManager);
    }
}
