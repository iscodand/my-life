using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;

namespace MyLifeApp.Application.Interfaces
{
    public interface IPostRepository : IBaseRepository
    {

        // TODO
        // => add partial update post

        public Task<GetAllPostsResponse> GetAllPosts();
        public Task<DetailPostResponse> GetPostById(Guid postId);
        public Task<BaseResponse> CreatePost(CreatePostRequest postRequest);
        public Task<BaseResponse> UpdatePost(Guid postId, UpdatePostRequest postRequest);
        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest);
        public Task<BaseResponse> LikePost(Guid postId);
        public Task<bool> PostExists(Guid postId);
    }
}
