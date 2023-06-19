using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<ICollection<Post>> GetPublicPostsAsync();
        public Task<Post> GetPostDetailsAsync(Guid postId);
        public Task<bool> PostExistsAsync(Guid postId);
        public Task<PostLike> GetPostLikeAsync(Profile profile, Post post);
        public Task<PostLike> AddPostLikeAsync(PostLike like);
        public Task RemovePostLikeAsync(PostLike like);
        public Task<bool> PostAlreadyLikedAsync(Profile profile, Post post);
        public Task<PostComment> GetPostCommentAsync(Guid commentId);
        public Task<PostComment> AddPostCommentAsync(PostComment comment);
        public Task DeletePostCommentAsync(PostComment comment);
        public Task<bool> PostCommentExistsAsync(Guid commentId);
    }
}
