using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;


namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IPostService
    {
        public Task<GetAllPostsResponse> GetPublicPostsAsync();
        public Task<DetailPostResponse> GetPostByIdAsync(Guid postId);
        public Task<BaseResponse> CreatePostAsync(CreatePostRequest request);
        public Task<BaseResponse> UpdatePostAsync(Guid postId, UpdatePostRequest request);
        public Task<BaseResponse> DeletePostAsync(Guid postId);
    }
}