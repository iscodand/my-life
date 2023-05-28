using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public readonly DbSet<Post> _posts;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _posts = context.Set<Post>();
        }

        public async Task<ICollection<Post>> GetPublicPostsAsync()
        {
            return await _posts.Where(p => p.IsPrivate == false)
                               .OrderBy(p => p.CreatedAt)
                               .Include(p => p.Profile)
                               .ThenInclude(p => p.User)
                               .ToListAsync();
        }

        public async Task<Post> GetPostDetailsAsync(Guid postId)
        {
            return await _posts.Where(p => p.Id == postId)
                               .Include(p => p.Profile)
                               .ThenInclude(p => p.User)
                               .FirstAsync();
        }

        public async Task<bool> PostExistsAsync(Guid postId)
        {
            return await _posts.AnyAsync(p => p.Id == postId);
        }

        public Task<BaseResponse> CommentPostAsync(Guid postId, CommentPostRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> LikePostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}