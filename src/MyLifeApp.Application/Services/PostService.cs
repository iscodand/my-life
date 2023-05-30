using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Domain.Entities;
using System.Security.Claims;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Services
{
    public class PostService : IPostService
    {
        public readonly IPostRepository _postRepository;
        public readonly IProfileRepository _profileRepository;
        public readonly IMapper _mapper;
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public PostService(IPostRepository postRepository,
                           IProfileRepository profileRepository,
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

        public async Task<GetAllPostsResponse> GetPublicPostsAsync()
        {
            ICollection<Post> posts = await _postRepository.GetPublicPostsAsync();
            ICollection<GetPostsResponse> postsMapper = _mapper.Map<ICollection<GetPostsResponse>>(posts);

            return new GetAllPostsResponse()
            {
                Posts = postsMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<DetailPostResponse> GetPostByIdAsync(Guid postId)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            Post post = await _postRepository.GetPostDetailsAsync(postId);
            Profile profile = post.Profile;

            GetPostsResponse postMapper = _mapper.Map<GetPostsResponse>(post);
            GetProfileResponse profileMapper = _mapper.Map<GetProfileResponse>(profile);

            return new DetailPostResponse()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profileMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        private async Task<Profile> GetAuthenticatedProfileAsync()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAllAsync();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);

            return authenticatedProfile;
        }

        private static bool IsPostCreator(Post post, Profile profile)
        {
            return post.Profile == profile;
        }

        public async Task<BaseResponse> CreatePostAsync(CreatePostRequest request)
        {
            Profile authenticatedProfile = await GetAuthenticatedProfileAsync();

            Post post = _mapper.Map<Post>(request);
            post.Profile = authenticatedProfile;
            await _postRepository.CreateAsync(post);

            PostAnalytics analytics = new()
            {
                Post = post,
                CommentsCount = 0,
                LikesCount = 0
            };

            return new BaseResponse()
            {
                Message = "Post successfuly created.",
                IsSuccess = true,
                StatusCode = 201
            };
        }

        public async Task<BaseResponse> UpdatePostAsync(Guid postId, UpdatePostRequest request)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            Post postToUpdate = await _postRepository.GetByIdAsync(postId);
            Profile authenticatedProfile = await GetAuthenticatedProfileAsync();

            if (!IsPostCreator(postToUpdate, authenticatedProfile))
            {
                return new BaseResponse()
                {
                    Message = "Only post creator can update the post.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            _mapper.Map(request, postToUpdate);
            await _postRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = "Post Successfuly Updated",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> DeletePostAsync(Guid postId)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            Post postToDelete = await _postRepository.GetByIdAsync(postId);
            Profile authenticatedProfile = await GetAuthenticatedProfileAsync();

            if (!IsPostCreator(postToDelete, authenticatedProfile))
            {
                return new BaseResponse()
                {
                    Message = "Only post creator can delete the post",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            await _postRepository.DeleteAsync(postToDelete);

            return new BaseResponse()
            {
                IsSuccess = true,
                StatusCode = 204
            };
        }
    }
}
