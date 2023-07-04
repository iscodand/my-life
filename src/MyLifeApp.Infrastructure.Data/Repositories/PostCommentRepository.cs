using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class PostCommentRepository : GenericRepository<PostComment>, IPostCommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<PostComment> _postComments;

        public PostCommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _postComments = context.Set<PostComment>();
        }

        public async Task<PostComment> CreatePostCommentAsync(PostComment postComment)
        {
            await _context.AddAsync(postComment);
            await _context.SaveChangesAsync();
            return postComment;
        }

        public async Task<ICollection<PostComment>> GetAllCommentsFromPost(int postId)
        {
            return await _postComments.Where(pc => pc.PostId == postId)
                                      .Include(pc => pc.Profile.User)
                                      .ToListAsync();
        }

        public async Task<bool> PostCommentExistsAsync(int commentId)
        {
            return await _postComments.AnyAsync(pc => pc.Id == commentId);
        }
    }
}