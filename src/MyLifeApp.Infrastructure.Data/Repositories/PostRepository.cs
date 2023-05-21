using AutoMapper;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public PostRepository(IHttpContextAccessor httpContext,
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        public async Task<GetAllPostsResponse> GetAllPosts()
        {
            ICollection<Post> posts = await _context.Posts.OrderBy(p => p.Id)
                                            .Include(p => p.Profile)
                                            .ThenInclude(p => p.User)
                                            .ToListAsync();

            ICollection<GetPostsResponse> postsMapper = _mapper.Map<ICollection<GetPostsResponse>>(posts);

            return new GetAllPostsResponse()
            {
                Posts = postsMapper,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<BaseResponse> CreatePost(CreatePostRequest postRequest)
        {
            if (postRequest == null)
            {
                return new BaseResponse
                {
                    Message = "Post cannot be null.",
                    IsSuccess = false
                };
            }

            User user = await GetAuthenticatedUser(_httpContext, _userManager);
            Domain.Entities.Profile profile = await _context.Profiles.FirstAsync(p => p.UserId == user.Id);

            Post post = _mapper.Map<Post>(postRequest);
            post.Profile = profile;

            _context.Add(post);

            if (_context.SaveChanges() == 0)
            {
                return new BaseResponse()
                {
                    Message = "Error while creating post.",
                    IsSuccess = false,
                };
            }

            return new BaseResponse()
            {
                Message = "Post successfuly created.",
                IsSuccess = true
            };
        }

        private bool IsPostCreator(Guid profileId, Post post)
        {
            return profileId == post.Profile.Id;
        }

        public async Task<BaseResponse> UpdatePost(Guid postId, UpdatePostRequest postRequest)
        {
            if (postRequest == null)
                return new BaseResponse()
                {
                    Message = "Post cannot be null.",
                    IsSuccess = false
                };

            Post post = await _context.Posts.Where(p => p.Id == postId)
                                            .Include(p => p.Profile)
                                            .FirstAsync();

            if (!IsPostCreator(post.Profile.Id, post))
            {
                return new BaseResponse()
                {
                    Message = "Only post creator can modify the post",
                    IsSuccess = false
                };
            }

            _mapper.Map(postRequest, post);
            await _context.SaveChangesAsync();

            return new BaseResponse()
            {
                Message = "Post successfuly updated.",
                IsSuccess = true
            };
        }

        public async Task<DetailPostResponse> GetPostById(Guid postId)
        {
            Post? post = await _context.Posts.Where(p => p.Id == postId)
                                             .Include(p => p.Profile)
                                             .ThenInclude(p => p.User)
                                             .FirstOrDefaultAsync();

            if (post == null)
            {
                return new DetailPostResponse()
                {
                    Message = "Post not found.",
                    IsSuccess = false
                };
            }

            GetProfileResponse profile = _mapper.Map<GetProfileResponse>(post.Profile);

            return new DetailPostResponse()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profile,
                Message = "Success",
                IsSuccess = true
            };
        }

        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> LikePost(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PostExists(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}
