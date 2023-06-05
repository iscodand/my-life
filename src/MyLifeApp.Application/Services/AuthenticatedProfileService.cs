using System.Security.Claims;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Services
{
    public class AuthenticatedProfileService : IAuthenticatedProfileService
    {
        private readonly UserManager<User> _manager;
        private readonly IHttpContextAccessor _context;
        private readonly IProfileRepository _profileRepository;

        public AuthenticatedProfileService(UserManager<User> manager,
                                           IHttpContextAccessor context,
                                           IProfileRepository profileRepository)
        {
            _manager = manager;
            _context = context;
            _profileRepository = profileRepository;
        }

        public async Task<Profile> GetAuthenticatedProfile()
        {
            ClaimsPrincipal userClaims = _context.HttpContext!.User;
            User? authenticatedUser = await _manager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAllAsync();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);

            return authenticatedProfile;
        }
    }
}