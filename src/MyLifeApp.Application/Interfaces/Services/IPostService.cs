using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;


namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IPostService
    {
        public Task<GetAllPostsResponse> GetPublicPostsAsync();
        public Task<DetailPostResponse> GetPostByIdAsync(int postId);
        public Task<BaseResponse> CreatePostAsync(CreatePostRequest request);
        public Task<BaseResponse> UpdatePostAsync(int postId, UpdatePostRequest request);
        public Task<BaseResponse> DeletePostAsync(int postId);
        public Task<BaseResponse> LikePostAsync(int postId);
        public Task<BaseResponse> UnlikePostAsync(int postId);
        public Task<BaseResponse> CommentPostAsync(int postId, CommentPostRequest request);
        public Task<BaseResponse> UpdateCommentAsync(int commentId, CommentPostRequest request);
        public Task<BaseResponse> DeleteCommentAsync(int commentId);
    }
}