using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public PostService(IPostRepository postRepository,
                           IMapper mapper,
                           IAuthenticatedProfileService authenticatedProfileService)
        {
            _postRepository = postRepository;
            _mapper = mapper;
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
            ICollection<GetPostCommentsDTO> commentsMapper = _mapper.Map<ICollection<GetPostCommentsDTO>>(post.PostComments);

            return new DetailPostResponse()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profileMapper,
                Likes = post.PostLikes.Count,
                Comments = commentsMapper,
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

        // ToDo => verify if post is private (as a bonus)
        public async Task<BaseResponse> CommentPostAsync(Guid postId, CommentPostRequest request)
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
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();

            PostComment comment = _mapper.Map<PostComment>(request);
            await _postRepository.AddPostCommentAsync(comment);

            return new BaseResponse()
            {
                Message = "Comment successfuly added",
                IsSuccess = true,
                StatusCode = 201
            };
        }

        public async Task<BaseResponse> UpdateCommentAsync(Guid commentId, CommentPostRequest request)
        {
            if (!await _postRepository.PostCommentExistsAsync(commentId))
            {
                return new BaseResponse()
                {
                    Message = "Comment not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            PostComment comment = await _postRepository.GetPostCommentAsync(commentId);
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();

            if (profile != comment.Profile)
            {
                return new BaseResponse()
                {
                    Message = "Only comment author can update the comment",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            PostComment commentToUpdate = _mapper.Map(request, comment);
            await _postRepository.SaveAsync();

            return new BaseResponse()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<BaseResponse> DeleteCommentAsync(Guid commentId)
        {
            if (!await _postRepository.PostCommentExistsAsync(commentId))
            {
                return new BaseResponse()
                {
                    Message = "Comment not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            PostComment comment = await _postRepository.GetPostCommentAsync(commentId);
            Profile profile = await _authenticatedProfileService.GetAuthenticatedProfile();

            // Isco => Only Comment Author and Post Author can delete the comment
            if (comment.Profile != profile && comment.Post.Profile != profile)
            {
                return new BaseResponse()
                {
                    Message = "Only Comment Author or Post Author can delete the comment",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            await _postRepository.DeletePostCommentAsync(comment);

            return new BaseResponse()
            {
                StatusCode = 204,
                IsSuccess = true
            };
        }
    }
}
