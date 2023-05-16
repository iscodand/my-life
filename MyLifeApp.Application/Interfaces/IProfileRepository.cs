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
        public Task<DetailProfileResponse> GetProfileByUsername(string profileUsername);
        public Task<GetTotalFollowersResponse> GetTotalFollowersByUsername(string profileUsername);
        public Task<BaseResponse> UpdateProfile(UpdateProfileRequest profileRequest);
        public Task<BaseResponse> FollowProfile(string username);
        public Task<BaseResponse> UnfollowProfile(string username);
    }
}
