using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<ICollection<Post>> GetPublicPostsAsync();
        public Task<Post> GetPostDetailsAsync(Guid postId);
        public Task<bool> PostExistsAsync(Guid postId);
        public Task<PostComment> AddCommentPostAsync(PostComment comment);
        public Task<PostLike> AddLikePostAsync(PostLike like);
        public Task RemoveLikePostAsync(PostLike like);
    }
}
