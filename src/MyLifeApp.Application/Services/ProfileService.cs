using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Identity.Infrastructure.Models;

using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;
using System.Security.Claims;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Requests.Profile;

namespace MyLifeApp.Application.Services
{
    public class ProfileService : IProfileService
    {
        public readonly IRefactorProfileRepository _profileRepository;
        public readonly IMapper _mapper;
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public ProfileService(IRefactorProfileRepository profileRepository,
                           IMapper mapper,
                           IHttpContextAccessor httpContext,
                           UserManager<User> userManager)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        public async Task<bool> CreateProfile(string userId)
        {
            Profile profile = new()
            {
                UserId = userId
            };

            ProfileAnalytics analytics = new()
            {
                Profile = profile,
                FollowersCount = 0,
                FollowingCount = 0
            };

            Profile? created = await _profileRepository.Create(profile);
            await _profileRepository.CreateProfileAnalytics(analytics);

            return created != null;
        }

        private async Task<Profile> _GetAuthenticatedProfile()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAll();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);

            return authenticatedProfile;
        }

        public async Task<DetailProfileResponse> GetAuthenticatedProfile()
        {
            Profile profile = await _GetAuthenticatedProfile();

            return new DetailProfileResponse()
            {
                Id = profile!.Id,
                Name = profile.User?.Name,
                Username = profile.User?.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<DetailProfileResponse> GetProfileByUsername(string username)
        {
            Profile? profile = await _profileRepository.GetProfileByUsername(username);

            return new DetailProfileResponse()
            {
                Id = profile!.Id,
                Name = profile.User?.Name,
                Username = profile.User?.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> UpdateProfile(UpdateProfileRequest request)
        {
            Profile profileToUpdate = await _GetAuthenticatedProfile();
            Profile profileMapper = _mapper.Map(request, profileToUpdate);
            await _profileRepository.Save();

            return new BaseResponse()
            {
                Message = "Profile successfuly updated",
                IsSuccess = true
            };
        }
    }
}