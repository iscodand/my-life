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
        public readonly DbSet<PostLike> _postLikes;
        public readonly DbSet<PostComment> _postComments;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _posts = context.Set<Post>();
            _postLikes = context.Set<PostLike>();
            _postComments = context.Set<PostComment>();
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
                               .Include(p => p.PostLikes)
                               .Include(p => p.PostComments)
                               .FirstAsync();
        }

        public async Task<bool> PostExistsAsync(Guid postId)
        {
            return await _posts.AnyAsync(p => p.Id == postId);
        }

        public async Task<PostComment> AddCommentPostAsync(PostComment comment)
        {
            await _postComments.AddAsync(comment);
            await base.SaveAsync();
            return comment;
        }

        public async Task<PostLike> AddLikePostAsync(PostLike like)
        {
            await _postLikes.AddAsync(like);
            await base.SaveAsync();
            return like;
        }
        
        public async Task RemoveLikePostAsync(PostLike like)
        {
            _postLikes.Remove(like);
            await base.SaveAsync();
        }
    }
}