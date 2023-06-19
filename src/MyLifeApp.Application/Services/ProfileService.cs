using AutoMapper;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public ProfileService(IProfileRepository profileRepository,
                           IMapper mapper,
                           IAuthenticatedProfileService authenticatedProfileService)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
            _authenticatedProfileService = authenticatedProfileService;
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

        public async Task<DetailProfileResponse> GetAuthenticatedProfileAsync()
        {
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();

            return new DetailProfileResponse()
            {
                Id = profile!.Id,
                Name = profile.User?.Name,
                Username = profile.User?.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<DetailProfileResponse> GetProfileByUsernameAsync(string username)
        {
            Profile? profile = await _profileRepository.GetProfileByUsernameAsync(username);

            if (profile == null)
            {
                return new DetailProfileResponse()
                {
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            return new DetailProfileResponse()
            {
                Id = profile!.Id,
                Name = profile.User?.Name,
                Username = profile.User?.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> UpdateProfileAsync(UpdateProfileRequest request)
        {
            Profile profileToUpdate = await _authenticatedProfileService.GetAuthenticatedProfile();
            Profile profileMapper = _mapper.Map(request, profileToUpdate);
            await _profileRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = "Profile successfuly updated",
                IsSuccess = true,
                StatusCode = 200
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
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            ICollection<ProfileFollower> followings = await _profileRepository.GetProfileFollowingsAsync(profile);
            ICollection<Profile> profileFollowings = followings.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowingsMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowings);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowingsMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
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
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            ICollection<ProfileFollower> followers = await _profileRepository.GetProfileFollowersAsync(profile);
            ICollection<Profile> profileFollowers = followers.Select(f => f.Follower).ToList();
            ICollection<GetProfileResponse> profileFollowersMapper = _mapper.Map<ICollection<GetProfileResponse>>(profileFollowers);

            return new GetFollowingsResponse()
            {
                Profiles = profileFollowersMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> FollowProfileAsync(string username)
        {
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();
            Profile follower = await _profileRepository.GetProfileByUsernameAsync(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            if (await _profileRepository.ProfileFollowerExistsAsync(profile, follower))
            {
                return new BaseResponse()
                {
                    Message = "You already follow this profile.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            ProfileFollower follow = new()
            {
                Profile = profile,
                Follower = follower
            };

            await _profileRepository.AddProfileFollower(follow);

            return new BaseResponse()
            {
                Message = $"Now you follow {follower.User?.UserName}",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> UnfollowProfileAsync(string username)
        {
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();
            Profile follower = await _profileRepository.GetProfileByUsernameAsync(username);

            if (follower == null)
            {
                return new BaseResponse()
                {
                    Message = "Profile not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            if (!await _profileRepository.ProfileFollowerExistsAsync(profile, follower))
            {
                return new BaseResponse()
                {
                    Message = "You not follow this profile.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            ProfileFollower unfollow = await _profileRepository.GetProfileFollowerAsync(profile, follower);

            await _profileRepository.RemoveProfileFollower(unfollow);

            return new BaseResponse()
            {
                Message = $"Successfuly Unfollow {follower.User?.UserName}",
                IsSuccess = true,
                StatusCode = 200
            };
        }
    }
}