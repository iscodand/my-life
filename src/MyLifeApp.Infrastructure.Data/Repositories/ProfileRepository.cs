using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class ProfileRepository : BaseRepository, IProfileRepository
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ProfileRepository(IHttpContextAccessor httpContext,
            UserManager<User> userManager,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _httpContext = httpContext;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        // TO-DO
        // refactor => verify the best way to create a profile
        public async Task<bool> RegisterProfile(string userId)
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

            await _context.Profiles.AddAsync(profile);
            await _context.ProfileAnalytics.AddAsync(analytics);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DetailProfileResponse> GetAuthenticatedProfile()
        {
            User authenticatedUser = await GetAuthenticatedUser(_httpContext, _userManager);
            Profile? profile = await _context.Profiles.FirstAsync(p => p.UserId == authenticatedUser!.Id);

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

        public async Task<DetailProfileResponse> GetProfile(string profileUsername)
        {
            Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);

            if (profile == null)
            {
                return new DetailProfileResponse()
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }

            User user = await _context.Users.FirstAsync(u => u.UserName == profileUsername);

            return new DetailProfileResponse()
            {
                Id = profile.Id,
                Name = profile.User!.Name,
                Username = profile.User.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> UpdateProfile(UpdateProfileRequest profileRequest)
        {
            if (profileRequest == null)
                return new BaseResponse()
                {
                    Message = "User cannot be null.",
                    IsSuccess = false
                };

            User? authenticatedUser = await _userManager.GetUserAsync(_httpContext.HttpContext!.User);
            Profile profile = _context.Profiles.First(u => u.UserId == authenticatedUser!.Id);
            _mapper.Map(profileRequest, profile);
            await _context.SaveChangesAsync();

            return new BaseResponse()
            {
                Message = "User successfuly updated.",
                IsSuccess = true
            };
        }

        private List<Profile> GetTotalProfileFollowers(Profile profile)
        {
            List<ProfileFollower> profileFollowers = _context.ProfileFollowers.Where(pf => pf.FollowerId == profile.Id)
                                                     .Include(p => p.Follower.User)
                                                     .ToList();


            List<Profile> followersCollection = new();
            foreach (ProfileFollower followers in profileFollowers)
            {
                followersCollection.Add(followers.Follower);
            }

            return followersCollection;
        }

        public async Task<GetFollowingsResponse> GetProfileFollowers(string profileUsername)
        {
            Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }

            ICollection<GetProfileResponse> followersResponseMapper = _mapper.Map<ICollection<GetProfileResponse>>(GetTotalProfileFollowers(profile));

            return new GetFollowingsResponse()
            {
                Total = followersResponseMapper.Count,
                Profiles = followersResponseMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        // TO-DO
        // => refactor
        // => maybe creating a BaseProfileFollowers/Following and avoid Code Redundance
        private List<Profile> GetTotalProfileFollowings(Profile profile)
        {
            List<ProfileFollower> profileFollowings = _context.ProfileFollowers.Where(pf => pf.ProfileId == profile.Id)
                                                      .Include(pf => pf.Follower.User)
                                                      .ToList();

            List<Profile> followingCollection = new();
            foreach (ProfileFollower follower in profileFollowings)
            {
                followingCollection.Add(follower.Follower);
            }

            return followingCollection;
        }

        public async Task<GetFollowingsResponse> GetProfileFollowings(string profileUsername)
        {
            Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);

            if (profile == null)
            {
                return new GetFollowingsResponse()
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            ICollection<GetProfileResponse> followingsResponseMapper = _mapper.Map<ICollection<GetProfileResponse>>(GetTotalProfileFollowings(profile));

            return new GetFollowingsResponse()
            {
                Total = followingsResponseMapper.Count,
                Profiles = followingsResponseMapper,
                Message = "Testing",
                IsSuccess = true
            };
        }

        private async Task<bool> AlreadyFollowProfile(User authenticatedUser, Profile followerProfile)
        {
            Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);
            List<ProfileFollower> authenticatedUserFollowers = _context.ProfileFollowers.Where(pf => pf.ProfileId == authenticatedUserProfile!.Id).ToList();
            bool alreadyFollow = authenticatedUserFollowers.Any(uf => uf.FollowerId == followerProfile.Id);
            return alreadyFollow;
        }

        private bool IsSelfFollowing(User authenticatedUser, Profile followerProfile)
        {
            return authenticatedUser.Id == followerProfile?.UserId;
        }

        private void UpdateProfileAnalytics(Profile profile, Profile follower, string action)
        {
            ProfileAnalytics profileAnalytics = _context.ProfileAnalytics.Where(a => a.Profile == profile)
                                                                               .First();
            ProfileAnalytics followerAnalytics = _context.ProfileAnalytics.Where(a => a.Profile == follower)
                                                                                .First();

            if (action == "follow")
            {
                profileAnalytics.FollowingCount += 1;
                followerAnalytics.FollowersCount += 1;
            }
            else if (action == "unfollow")
            {
                profileAnalytics.FollowingCount -= 1;
                followerAnalytics.FollowersCount -= 1;
            }
            else
            {
                throw new InvalidOperationException("Action must be follow or unfollow.");
            }

            _context.ProfileAnalytics.Update(profileAnalytics);
            _context.ProfileAnalytics.Update(followerAnalytics);
        }

        public async Task<BaseResponse> FollowProfile(string profileUsername)
        {
            User authenticatedUser = await GetAuthenticatedUser(_httpContext, _userManager);
            Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);

            Profile? followerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);
            if (followerProfile == null)
            {
                return new BaseResponse()
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            bool alreadyFollow = await AlreadyFollowProfile(authenticatedUser, followerProfile);
            bool isSelfFollowing = IsSelfFollowing(authenticatedUser, followerProfile);

            if (isSelfFollowing)
            {
                return new BaseResponse()
                {
                    Message = "Sorry, you cannot follow your own account. Please select another account to follow.",
                    IsSuccess = false
                };
            }

            if (!alreadyFollow)
            {
                ProfileFollower profileFollower = new()
                {
                    Profile = authenticatedUserProfile!,
                    Follower = followerProfile
                };

                UpdateProfileAnalytics(authenticatedUserProfile!, followerProfile, "follow");
                await _context.ProfileFollowers.AddAsync(profileFollower);
                await _context.SaveChangesAsync();

                return new BaseResponse()
                {
                    Message = "Follow successfuly.",
                    IsSuccess = true
                };
            }

            return new BaseResponse()
            {
                Message = "You already follow this profile.",
                IsSuccess = false
            };
        }

        public async Task<BaseResponse> UnfollowProfile(string profileUsername)
        {
            User authenticatedUser = await GetAuthenticatedUser(_httpContext, _userManager);
            Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);

            Profile? followerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);
            if (followerProfile == null)
            {
                return new BaseResponse()
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            bool alreadyFollow = await AlreadyFollowProfile(authenticatedUser, followerProfile);
            bool isSelfFollowing = IsSelfFollowing(authenticatedUser, followerProfile);

            if (isSelfFollowing)
            {
                return new BaseResponse()
                {
                    Message = "Sorry, you cannot follow your own account. Please select another account to follow.",
                    IsSuccess = false
                };
            }

            if (alreadyFollow)
            {
                ProfileFollower? profileFollower = await _context.ProfileFollowers.FirstOrDefaultAsync(
                    pf => pf.Profile == authenticatedUserProfile
                    && pf.Follower == followerProfile);

                _context.ProfileFollowers.Remove(profileFollower!);

                UpdateProfileAnalytics(authenticatedUserProfile!, followerProfile, "unfollow");
                await _context.SaveChangesAsync();
                return new BaseResponse()
                {
                    Message = "Unfollow successfuly.",
                    IsSuccess = true
                };
            }

            return new BaseResponse()
            {
                Message = "You must to be a follower of this profile to unfollow.",
                IsSuccess = false
            };
        }
    }
}
