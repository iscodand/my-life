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
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public PostService(IPostRepository postRepository,
                           IProfileRepository profileRepository,
                           IMapper mapper,
                           IHttpContextAccessor httpContext,
                           UserManager<User> userManager,
                           IAuthenticatedProfileService authenticatedProfileService)
        {
            _postRepository = postRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
            _authenticatedProfileService = authenticatedProfileService;
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

        // TODO => add validation for private posts only for posts owners
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

        private static bool IsPostCreator(Post post, Profile profile)
        {
            return post.Profile == profile;
        }

        public async Task<BaseResponse> CreatePostAsync(CreatePostRequest request)
        {
            Profile authenticatedProfile = await _authenticatedProfileService.GetAuthenticatedProfile();

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
            Profile authenticatedProfile = await _authenticatedProfileService.GetAuthenticatedProfile();

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
            Profile authenticatedProfile = await _authenticatedProfileService.GetAuthenticatedProfile();

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

        public async Task<BaseResponse> LikePostAsync(Guid postId)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return new BaseResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            Post post = await _postRepository.GetPostDetailsAsync(postId);
            Profile authenticatedProfile = await _authenticatedProfileService.GetAuthenticatedProfile();

            if (await _postRepository.PostAlreadyLikedAsync(authenticatedProfile, post))
            {
                return new BaseResponse()
                {
                    Message = "You already liked this post.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            PostLike like = new()
            {
                Profile = authenticatedProfile,
                Post = post
            };

            await _postRepository.AddPostLikeAsync(like);

            return new BaseResponse()
            {
                Message = "Post successfuly liked",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> UnlikePostAsync(Guid postId)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return new BaseResponse()
                {
                    Message = "Post not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            Post post = await _postRepository.GetPostDetailsAsync(postId);
            Profile authenticatedProfile = await _authenticatedProfileService.GetAuthenticatedProfile();

            PostLike? postToUnlike = await _postRepository.GetPostLikeAsync(authenticatedProfile, post);
            
            if (!await _postRepository.PostAlreadyLikedAsync(authenticatedProfile, post))
            {
                return new BaseResponse()
                {
                    Message = "You doens't like this post yet.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            await _postRepository.RemovePostLikeAsync(postToUnlike);

            return new BaseResponse()
            {
                Message = "Post successfuly unliked",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public Task<BaseResponse> CommentPostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> UpdateCommentAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> DeleteCommentAsync(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}
