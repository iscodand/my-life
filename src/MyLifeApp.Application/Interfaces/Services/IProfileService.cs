using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IProfileService
    {
        public Task<DetailProfileResponse> GetAuthenticatedProfile();
        public Task<DetailProfileResponse> GetProfileByUsername(string username);
        public Task<GetFollowingsResponse> GetProfileFollowings(string username);
        public Task<GetFollowingsResponse> GetProfileFollowers(string username);
        public Task<BaseResponse> FollowProfile(string username);
        public Task<BaseResponse> UnfollowProfile(string username);
        public Task<bool> CreateProfile(string userId);
        public Task<BaseResponse> UpdateProfile(UpdateProfileRequest request);
    }
}