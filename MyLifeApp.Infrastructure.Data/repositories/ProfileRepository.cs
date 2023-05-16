using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile.Followers;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;
using System.Diagnostics;
using System.Security.Claims;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class ProfileRepository : IProfileRepository
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

        private async Task<User> GetAuthenticatedUser()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            return authenticatedUser!;
        }

        // TO-DO
        // Refactor => verify the best way to create a profile
        public bool RegisterProfile(string userId)
        {
            try
            {
                Domain.Entities.Profile profile = new()
                {
                    UserId = userId
                };
                _context.Add(profile);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        public async Task<DetailProfileResponse> GetAuthenticatedProfile()
        {
            User authenticatedUser = await GetAuthenticatedUser();
            Domain.Entities.Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser!.Id);

            return new DetailProfileResponse()
            {
                Id = profile!.Id,
                Name = profile.User.Name,
                Username = profile.User.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<DetailProfileResponse> GetProfileByUsername(string profileUsername)
        {
            Domain.Entities.Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == profileUsername);

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
                Name = profile.User.Name,
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
            Domain.Entities.Profile profile = _context.Profiles.First(u => u.UserId == authenticatedUser!.Id);
            _mapper.Map(profileRequest, profile);
            await _context.SaveChangesAsync();

            return new BaseResponse()
            {
                Message = "User successfuly updated.",
                IsSuccess = true
            };
        }

        private async Task<bool> AlreadyFollowProfile(User authenticatedUser, Domain.Entities.Profile followerProfile)
        {
            Domain.Entities.Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);
            List<ProfileFollower> authenticatedUserFollowers = _context.ProfileFollowers.Where(pf => pf.ProfileId == authenticatedUserProfile!.Id).ToList();
            bool alreadyFollow = authenticatedUserFollowers.Any(uf => uf.FollowerId == followerProfile.Id);
            return alreadyFollow;
        }

        // TO-DO
        // => add ProfileAnalytics followers counter support
        // => add ProfileAnalytics following counter support
        // => refactor
        public async Task<BaseResponse> FollowProfile(string username)
        {
            User authenticatedUser = await GetAuthenticatedUser();
            Domain.Entities.Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);

            Domain.Entities.Profile? followerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == username);
            if (followerProfile == null)
            {
                return new BaseResponse()
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            bool alreadyFollow = await AlreadyFollowProfile(authenticatedUser, followerProfile);

            if (!alreadyFollow)
            {
                ProfileFollower profileFollower = new()
                {
                    Profile = authenticatedUserProfile!,
                    Follower = followerProfile
                };

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

        public async Task<BaseResponse> UnfollowProfile(string username)
        {
            User authenticatedUser = await GetAuthenticatedUser();
            Domain.Entities.Profile? authenticatedUserProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser.Id);

            Domain.Entities.Profile? followerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.User.UserName == username);
            if (followerProfile == null)
            {
                return new BaseResponse()
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            bool alreadyFollow = await AlreadyFollowProfile(authenticatedUser, followerProfile);

            if (alreadyFollow)
            {
                ProfileFollower? profileFollower = await _context.ProfileFollowers.FirstOrDefaultAsync(
                    pf => pf.Profile == authenticatedUserProfile
                    && pf.Follower == followerProfile);
                _context.ProfileFollowers.Remove(profileFollower!);
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

        public Task<GetTotalFollowersResponse> GetTotalFollowersByUsername(string profileUsername)
        {
            throw new NotImplementedException();
        }
    }
}
