using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
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
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            Domain.Entities.Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == authenticatedUser!.Id);
            
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

        public Task<BaseResponse> FollowProfile(FollowProfileRequest profileRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> FollowUnfollowProfile(FollowProfileRequest profileRequest)
        {
            throw new NotImplementedException();
        }
    }
}
