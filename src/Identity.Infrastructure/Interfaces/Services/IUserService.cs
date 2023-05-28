using Identity.Infrastructure.DTOs.Request;
using Identity.Infrastructure.DTOs.Response;

namespace Identity.Infrastructure.Interfaces.Services
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
    }
}
