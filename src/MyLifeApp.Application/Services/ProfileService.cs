using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Repositories;
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

        public async Task<bool> CreateProfileAsync(string userId)
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

            Profile? created = await _profileRepository.CreateAsync(profile);
            await _profileRepository.CreateProfileAnalyticsAsync(analytics);

            return created != null;
        }

        private async Task<Profile> _GetAuthenticatedProfileAsync()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAllAsync();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);

            return authenticatedProfile;
        }

        public async Task<DetailProfileResponse> GetAuthenticatedProfileAsync()
        {
            Profile profile = await _GetAuthenticatedProfileAsync();

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

        public async Task<DetailProfileResponse> GetProfileByUsernameAsync(string username)
        {
            Profile? profile = await _profileRepository.GetProfileByUsernameAsync(username);

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

        public async Task<BaseResponse> UpdateProfileAsync(UpdateProfileRequest request)
        {
            Profile profileToUpdate = await _GetAuthenticatedProfileAsync();
            Profile profileMapper = _mapper.Map(request, profileToUpdate);
            await _profileRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = "Profile successfuly updated",
                IsSuccess = true
            };
        }

        public async Task<GetFollowingsResponse> GetProfileFollowingsAsync(string username)
        {
            Profile profile = await _profileRepository.GetProfileByUsernameAsync(username);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = true
                };
            }

            ICollection<ProfileFollower> followings = await _profileRepository.GetProfileFollowingsAsync(profile);
            ICollection<Profile> profileFollowings = followings.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowingsMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowings);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowingsMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<GetFollowingsResponse> GetProfileFollowersAsync(string username)
        {
            Profile profile = await _profileRepository.GetProfileByUsernameAsync(username);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> followers = await _profileRepository.GetProfileFollowersAsync(profile);
            ICollection<Profile> profileFollowers = followers.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowersMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowers);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowersMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> FollowProfileAsync(string username)
        {
            Profile profile = await _GetAuthenticatedProfileAsync();
            Profile follower = await _profileRepository.GetProfileByUsernameAsync(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> profileFollowings = await _profileRepository.GetProfileFollowingsAsync(profile);

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

            profile.ProfileFollowers!.Add(follow);
            await _profileRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = $"Now you follow {follower.User?.UserName}",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> UnfollowProfileAsync(string username)
        {
            Profile profile = await _GetAuthenticatedProfileAsync();
            Profile follower = await _profileRepository.GetProfileByUsernameAsync(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false
                };
            }

            ICollection<ProfileFollower> profileFollowings = await _profileRepository.GetProfileFollowingsAsync(profile);

            if (!profileFollowings.Any(pf => pf.Follower == follower))
            {
                return new BaseResponse()
                {
                    Message = "You not follow this profile.",
                    IsSuccess = false
                };
            }

            ProfileFollower unfollow = profileFollowings.Where(pf => pf.Follower == follower).First();

            profile.ProfileFollowers!.Remove(unfollow);
            await _profileRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = $"Successfuly Unfollow {follower.User?.UserName}",
                IsSuccess = true
            };
        }
    }
}