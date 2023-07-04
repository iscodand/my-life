using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<ICollection<Post>> GetPublicPostsAsync();
        public Task<Post> GetPostDetailsAsync(int postId);
        public Task<bool> PostExistsAsync(int postId);
     
        // ToDo => pull apart PostLike for PostRepository
        public Task<PostLike> GetPostLikeAsync(Profile profile, Post post);
        public Task<PostLike> AddPostLikeAsync(PostLike like);
        public Task RemovePostLikeAsync(PostLike like);
        public Task<bool> PostAlreadyLikedAsync(Profile profile, Post post);
    }
}
