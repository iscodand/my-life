using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Responses;

namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IProfileService
    {
        public Task<DetailProfileResponse> GetAuthenticatedProfile();
        public Task<DetailProfileResponse> GetProfileByUsername(string username);
        public Task<bool> CreateProfile(string userId);
        public Task<BaseResponse> UpdateProfile(UpdateProfileRequest request);
    }
}