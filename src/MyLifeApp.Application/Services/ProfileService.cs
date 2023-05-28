using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Domain.Entities;
using System.Security.Claims;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Services
{
    public class ProfileService : IProfileService
    {
        public readonly IProfileRepository _profileRepository;
        public readonly IMapper _mapper;
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public ProfileService(IProfileRepository profileRepository,
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

        public async Task<GetFollowingsResponse> GetProfileFollowings(string username)
        {
            Profile profile = await _profileRepository.GetProfileByUsername(username);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = true
                };
            }

            ICollection<ProfileFollower> followings = await _profileRepository.GetProfileFollowings(profile);
            ICollection<Profile> profileFollowings = followings.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowingsMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowings);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowingsMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<GetFollowingsResponse> GetProfileFollowers(string username)
        {
            Profile profile = await _profileRepository.GetProfileByUsername(username);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> followers = await _profileRepository.GetProfileFollowers(profile);
            ICollection<Profile> profileFollowers = followers.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowersMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowers);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowersMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> FollowProfile(string username)
        {
            Profile profile = await _GetAuthenticatedProfile();
            Profile follower = await _profileRepository.GetProfileByUsername(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> profileFollowings = await _profileRepository.GetProfileFollowings(profile);

            if (profileFollowings.Any(pf => pf.Follower == follower))
            {
                return new BaseResponse()
                {
                    Message = "You already follow this profile.",
                    IsSuccess = false
                };
            }

            ProfileFollower follow = new()
            {
                Profile = profile,
                Follower = follower
            };

            await _profileRepository.CreateProfileFollower(follow);

            return new BaseResponse()
            {
                Message = $"Now you follow {follower.User?.UserName}",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> UnfollowProfile(string username)
        {
            Profile profile = await _GetAuthenticatedProfile();
            Profile follower = await _profileRepository.GetProfileByUsername(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> profileFollowings = await _profileRepository.GetProfileFollowings(profile);

            if (!profileFollowings.Any(pf => pf.Follower == follower))
            {
                return new BaseResponse()
                {
                    Message = "You not follow this profile.",
                    IsSuccess = false
                };
            }

            ProfileFollower unfollow = profileFollowings.Where(pf => pf.Follower == follower).First();
            await _profileRepository.RemoveProfileFollower(unfollow);

            return new BaseResponse()
            {
                Message = $"Successfuly Unfollow {follower.User?.UserName}",
                IsSuccess = true
            };
        }
    }
}