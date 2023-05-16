using Identity.Infrastructure.DTOs.Request;
using Identity.Infrastructure.DTOs.Response;

namespace Identity.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        // TO-DO:
        // -> Add recover password
        // -> Add update user method
        // -> Add two-steps verification with e-mail

        public Task<RegisterUserResponse> Register(RegisterUserRequest userRequest);
        public Task<LoginUserResponse> Login(LoginUserRequest userRequest);
        public Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest tokenRequest);    
    }
}
