using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;


namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IPostService
    {
        public Task<GetAllPostsResponse> GetPublicPostsAsync();
        public Task<DetailPostResponse> GetPostByIdAsync(string postId);
        public Task<BaseResponse> CreatePostAsync(CreatePostRequest request);
        public Task<BaseResponse> UpdatePostAsync(string postId, UpdatePostRequest request);
        public Task<BaseResponse> DeletePostAsync(string postId);
        public Task<BaseResponse> LikePostAsync(string postId);
        public Task<BaseResponse> UnlikePostAsync(string postId);
        public Task<BaseResponse> CommentPostAsync(string postId, CommentPostRequest request);
        public Task<BaseResponse> UpdateCommentAsync(string commentId, CommentPostRequest request);
        public Task<BaseResponse> DeleteCommentAsync(string commentId);
    }
}