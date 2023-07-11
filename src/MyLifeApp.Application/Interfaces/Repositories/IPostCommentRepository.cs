using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostCommentRepository : IGenericRepository<PostComment>
    {
        public Task<ICollection<PostComment>> GetAllCommentsFromPost(int postId);
        public Task<bool> PostCommentExistsAsync(int commentId);
    }
}