using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.Models;

namespace MyLifeApp.Infrastructure.Identity.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public AuthenticatedUserService(UserManager<User> userManager,
                                           IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            return authenticatedUser!;
        }
    }
}