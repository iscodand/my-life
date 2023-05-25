using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces
{
    public interface IRefactorPostRepository : IGenericRepository<Post>
    {
        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest);
        public Task<BaseResponse> LikePost(Guid postId);
    }
}
