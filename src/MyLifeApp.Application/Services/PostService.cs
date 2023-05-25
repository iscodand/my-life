using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Identity.Infrastructure.Models;

using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;
using System.Security.Claims;

namespace MyLifeApp.Application.Services
{
    public class PostService : IPostService
    {
        public readonly IRefactorPostRepository _postRepository;
        public readonly IRefactorProfileRepository _profileRepository;
        public readonly IMapper _mapper;
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public PostService(IRefactorPostRepository postRepository,
                           IRefactorProfileRepository profileRepository,
                           IMapper mapper,
                           IHttpContextAccessor httpContext,
                           UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        private async Task<Profile> GetAuthenticatedProfile(IHttpContextAccessor context, UserManager<User> manager)
        {
            ClaimsPrincipal userClaims = context.HttpContext!.User;
            User? authenticatedUser = await manager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAll();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);
            return authenticatedProfile;
        }

        public async Task<BaseResponse> CreatePost(CreatePostRequest request)
        {
            Profile profile = await GetAuthenticatedProfile(_httpContext, _userManager);
            
            Post post = _mapper.Map<Post>(request);
            post.Profile = profile;
            await _postRepository.Create(post);

            PostAnalytics analytics = new()
            {
                Post = post,
                CommentsCount = 0,
                LikesCount = 0
            };

            return new BaseResponse()
            {
                Message = "Post successfuly created.",
                IsSuccess = true
            };
        }
    }
}
