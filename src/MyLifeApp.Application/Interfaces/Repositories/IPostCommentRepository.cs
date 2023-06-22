using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostCommentRepository : IGenericRepository<PostComment> 
    { 
        public Task<PostComment> CreatePostCommentAsync(PostComment postComment);
        public Task<ICollection<PostComment>> GetAllCommentsFromPost(string postId);
        public Task<bool> PostCommentExistsAsync(string commentId);
    }
}