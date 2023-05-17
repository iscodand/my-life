using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Responses;

namespace MyLifeApp.Application.Interfaces
{
    public interface IProfileRepository
    {
        // Implement =>
        // Get all Posts from Profile
        // Get all Followers from Profile
        // Get all Posts Liked from Profile
        // Get all Comments from Profile
        // Patch Update Profile

        public bool RegisterProfile(string userId);
        public Task<DetailProfileResponse> GetAuthenticatedProfile();
        public Task<DetailProfileResponse> GetProfile(string profileUsername);
        public Task<GetFollowingsResponse> GetProfileFollowings(string profileUsername);
        public Task<GetFollowingsResponse> GetProfileFollowers(string profileUsername);
        public Task<BaseResponse> UpdateProfile(UpdateProfileRequest profileRequest);
        public Task<BaseResponse> FollowProfile(string profileUsername);
        public Task<BaseResponse> UnfollowProfile(string profileUsername);
    }
}
