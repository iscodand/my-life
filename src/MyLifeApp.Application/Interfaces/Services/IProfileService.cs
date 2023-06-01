using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IProfileService
    {
        public Task<DetailProfileResponse> GetAuthenticatedProfileAsync();
        public Task<DetailProfileResponse> GetProfileByUsernameAsync(string username);
        public Task<GetFollowingsResponse> GetProfileFollowingsAsync(string username);
        public Task<GetFollowingsResponse> GetProfileFollowersAsync(string username);
        public Task<BaseResponse> FollowProfileAsync(string username);
        public Task<BaseResponse> UnfollowProfileAsync(string username);
        public Task<bool> CreateProfileAsync(string userId);
        public Task<BaseResponse> UpdateProfileAsync(UpdateProfileRequest request);
    }
}