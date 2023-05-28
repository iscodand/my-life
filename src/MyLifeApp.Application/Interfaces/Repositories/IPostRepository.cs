using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<ICollection<Post>> GetPublicPostsAsync();
        public Task<Post> GetPostDetailsAsync(Guid postId);
        public Task<bool> PostExistsAsync(Guid postId);
        public Task<BaseResponse> CommentPostAsync(Guid postId, CommentPostRequest postRequest);
        public Task<BaseResponse> LikePostAsync(Guid postId);
    }
}
