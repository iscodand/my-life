using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses;


namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IPostService
    {
        public Task<GetAllPostsResponse> GetPublicPosts();
        public Task<DetailPostResponse> GetPostById(Guid postId);
        public Task<BaseResponse> CreatePost(CreatePostRequest request);
        public Task<BaseResponse> UpdatePost(Guid postId, UpdatePostRequest request);
        public Task<BaseResponse> DeletePost(Guid postId);
    }
}