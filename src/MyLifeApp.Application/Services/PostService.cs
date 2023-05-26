using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using Identity.Infrastructure.Models;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Services
{
    public class PostService : IPostService
    {
        public readonly IPostRepository _postRepository;
        public readonly IRefactorProfileRepository _profileRepository;
        public readonly IMapper _mapper;
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public PostService(IPostRepository postRepository,
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

        public async Task<GetAllPostsResponse> GetPublicPosts()
        {
            ICollection<Post> posts = await _postRepository.GetPublicPosts();
            ICollection<GetPostsResponse> postsMapper = _mapper.Map<ICollection<GetPostsResponse>>(posts);

            return new GetAllPostsResponse()
            {
                Posts = postsMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<DetailPostResponse> GetPostById(Guid postId)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false
                };
            }

            Post post = await _postRepository.GetPostDetails(postId);
            Profile profile = post.Profile;

            GetPostsResponse postMapper = _mapper.Map<GetPostsResponse>(post);
            GetProfileResponse profileMapper = _mapper.Map<GetProfileResponse>(profile);

            return new DetailPostResponse()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profileMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        private async Task<Profile> GetAuthenticatedProfile()
        {
            ClaimsPrincipal userClaims = _httpContext.HttpContext!.User;
            User? authenticatedUser = await _userManager.GetUserAsync(userClaims);
            ICollection<Profile> profiles = await _profileRepository.GetAll();
            Profile authenticatedProfile = profiles.First(p => p.UserId == authenticatedUser!.Id);

            return authenticatedProfile;
        }

        private bool IsPostCreator(Post post, Profile profile)
        {
            return post.Profile == profile;
        }

        public async Task<BaseResponse> CreatePost(CreatePostRequest request)
        {
            Profile authenticatedProfile = await GetAuthenticatedProfile();

            Post post = _mapper.Map<Post>(request);
            post.Profile = authenticatedProfile;
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

        public async Task<BaseResponse> UpdatePost(Guid postId, UpdatePostRequest request)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false
                };
            }

            Post postToUpdate = await _postRepository.GetById(postId);
            Profile authenticatedProfile = await GetAuthenticatedProfile();

            if (!IsPostCreator(postToUpdate, authenticatedProfile))
            {
                return new BaseResponse()
                {
                    Message = "Only post creator can update the post.",
                    IsSuccess = false
                };
            }

            _mapper.Map(request, postToUpdate);
            await _postRepository.Save();

            return new BaseResponse()
            {
                Message = "Post Successfuly Updated",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> DeletePost(Guid postId)
        {
            if (!await _postRepository.PostExists(postId))
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false
                };
            }

            Post postToDelete = await _postRepository.GetById(postId);
            Profile authenticatedProfile = await GetAuthenticatedProfile();

            if (!IsPostCreator(postToDelete, authenticatedProfile))
            {
                return new BaseResponse()
                {
                    Message = "Only post creator can delete the post",
                    IsSuccess = false
                };
            }

            await _postRepository.Delete(postToDelete);

            return new BaseResponse()
            {
                IsSuccess = true
            };
        }
    }
}
