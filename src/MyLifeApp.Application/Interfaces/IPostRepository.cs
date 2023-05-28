using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<ICollection<Post>> GetPublicPosts();
        public Task<Post> GetPostDetails(Guid postId);
        public Task<bool> PostExists(Guid postId);
        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest);
        public Task<BaseResponse> LikePost(Guid postId);
    }
}
