using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Application.Interfaces;
using System.Security.Claims;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public abstract class BaseRepository : IBaseRepository
    {
        public async Task<User> GetAuthenticatedUser(IHttpContextAccessor httpContext, UserManager<User> userManager)
        {
            ClaimsPrincipal userClaims = httpContext.HttpContext!.User;
            User? authenticatedUser = await userManager.GetUserAsync(userClaims);
            return authenticatedUser!;
        }
    }
}
