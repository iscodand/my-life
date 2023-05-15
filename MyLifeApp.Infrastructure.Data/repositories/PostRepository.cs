using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Interfaces;
using AutoMapper;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<BaseResponse> CreatePost(CreatePostRequest postRequest)
        {
            Post post = _mapper.Map<Post>(postRequest);
            return null;
        }
        
        public Task<BaseResponse> CommentPost(CommentPostRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<GetPostsResponse>> GetAllPosts()
        {
            throw new NotImplementedException();
        }

        public Task<DetailPostResponse> GetPostById(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> LikePost()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> UpdatePost(UpdatePostRequest postRequest)
        {
            throw new NotImplementedException();
        }
    }
}
