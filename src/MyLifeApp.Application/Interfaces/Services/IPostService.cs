using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses;


namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IPostService
    {
        public Task<BaseResponse> CreatePost(CreatePostRequest request);
    }
}