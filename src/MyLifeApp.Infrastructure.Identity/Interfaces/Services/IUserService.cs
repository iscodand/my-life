using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.DTOs.Response;

namespace MyLifeApp.Infrastructure.Identity.Interfaces.Services
{
    public interface IUserService
    {
        // TO-DO:
        // -> Add recover password
        // -> Add update user method
        // -> Add two-steps verification with e-mail

        public Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest userRequest);
        public Task<LoginUserResponse> LoginAsync(LoginUserRequest userRequest);
        public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest tokenRequest);
        public Task<BaseResponse> UpdatePasswordAsync(UpdatePasswordRequest request);
        public Task<BaseResponse> ForgetPasswordAsync(ForgetPasswordRequest request);
        public Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
