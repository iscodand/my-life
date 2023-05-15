using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;

namespace MyLifeApp.Application.Interfaces
{
    public interface IPostRepository
    {
        public Task<ICollection<GetPostsResponse>> GetAllPosts();
        public Task<DetailPostResponse> GetPostById(int postId);
        public Task<BaseResponse> CreatePost(CreatePostRequest postRequest);
        public Task<BaseResponse> UpdatePost(UpdatePostRequest postRequest);
    }
}
