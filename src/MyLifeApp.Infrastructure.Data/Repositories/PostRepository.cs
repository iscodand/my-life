using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Requests.Post;
using Microsoft.EntityFrameworkCore;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public readonly DbSet<Post> _posts;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _posts = context.Set<Post>();
        }

        public async Task<ICollection<Post>> GetPublicPosts()
        {
            return await _posts.Where(p => p.IsPrivate == false)
                               .OrderBy(p => p.CreatedAt)
                               .Include(p => p.Profile)
                               .ThenInclude(p => p.User)
                               .ToListAsync();
        }

        public async Task<Post> GetPostDetails(Guid postId)
        {
            return await _posts.Where(p => p.Id == postId)
                               .Include(p => p.Profile)
                               .ThenInclude(p => p.User)
                               .FirstAsync();
        }

        public async Task<bool> PostExists(Guid postId)
        {
            return await _posts.AnyAsync(p => p.Id == postId);
        }

        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> LikePost(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}