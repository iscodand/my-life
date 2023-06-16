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


        // ToDo => create a repository for PostLike entity to avoid code redundance 
        public async Task<PostLike> GetPostLikeAsync(Profile profile, Post post)
        {
            return await _postLikes.FirstAsync(p => p.Profile == profile && p.Post == post);
        }

        public async Task<PostLike> AddPostLikeAsync(PostLike like)
        {
            await _postLikes.AddAsync(like);
            await base.SaveAsync();
            return like;
        }

        public async Task RemovePostLikeAsync(PostLike like)
        {
            _postLikes.Remove(like);
            await base.SaveAsync();
        }

        public async Task<bool> PostAlreadyLikedAsync(Profile profile, Post post)
        {
            return await _postLikes.AnyAsync(p => p.Profile == profile && p.Post == post);
        }

        public async Task<PostComment> GetPostCommentAsync(Guid commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<PostComment> AddCommentPostAsync(PostComment comment)
        {
            await _postComments.AddAsync(comment);
            await base.SaveAsync();
            return comment;
        }

        public async Task<PostComment> UpdateCommentPostAsync(Guid commentId, PostComment comment)
        {
            throw new NotImplementedException();
        }

        public Task<PostComment> DeleteCommentPostAsync(Guid commentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PostCommentExistsAsync(Guid commentId)
        {
            throw new NotImplementedException();
        }
    }
}