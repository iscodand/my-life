using Identity.Infrastructure.Data.DTOs.Request;
using Identity.Infrastructure.Data.DTOs.Response;
using Identity.Infrastructure.Interfaces;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly ITokenRepository _tokenRepository;
        public readonly UserManager<User> _userManager;
        public readonly IConfiguration _configuration;

        public UserRepository(ITokenRepository tokenRepository,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _tokenRepository = tokenRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<BaseResponse> Register(RegisterUserRequest userRequest)
        {
            User user = new()
            {
                Name = userRequest.Name,
                UserName = userRequest.Username,
                Email = userRequest.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, userRequest.Password);

            if (!result.Succeeded)
            {
                return new BaseResponse()
                {
                    Message = "Error while creating user",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new BaseResponse()
            {
                Message = "User successfuly created",
                IsSuccess = true,
            };
        }

        public async Task<LoginUserResponse> Login(LoginUserRequest userRequest)
        {
            User user = await _userManager.FindByNameAsync(userRequest.Username);

            if (user == null)
            {
                return new LoginUserResponse()
                {
                    Message = "Username doens't exists. Verify and try again.",
                    IsSuccess = false
                };
            }

            bool verifyPassword = await _userManager.CheckPasswordAsync(user, userRequest.Password);

            if (!verifyPassword)
            {
                return new LoginUserResponse()
                {
                    Message = "Password invalid. Verify and try again.",
                    IsSuccess = false
                };
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Generating tokens
            JwtSecurityToken accessToken = _tokenRepository.GenerateAccessToken(claims);
            string refreshToken = _tokenRepository.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWTSettings:RefreshTokenValidityInMinutes"],
                out int refreshTokenValidityTime);

            user.RefreshToken = refreshToken;
            user.RefreshTokenValidityTime = DateTime.Now.AddMinutes(refreshTokenValidityTime);

            await _userManager.UpdateAsync(user);

            string accessTokenAsString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return new LoginUserResponse()
            {
                Message = "Login Successfuly",
                AccessToken = accessTokenAsString,
                RefreshToken = refreshToken,
                IsSuccess = true
            };
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest tokenRequest)
        {
            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            ClaimsPrincipal principal = _tokenRepository.GetPrincipalForExpiredToken(accessToken);

            if (principal == null)
            {
                return new RefreshTokenResponse()
                {
                    Message = "Invalid access/refresh token.",
                    IsSuccess = false
                };
            }

            User user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenValidityTime <= DateTime.Now)
            {
                return new RefreshTokenResponse()
                {
                    Message = "Invalid access/refresh token",
                    IsSuccess = false
                };
            }

            JwtSecurityToken newAccessToken = _tokenRepository.GenerateAccessToken(principal.Claims.ToList());
            string newRefreshToken = _tokenRepository.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWTSettings:RefreshTokenValidityInMinutes"],
                out int refreshTokenValidityTime);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenValidityTime = DateTime.Now.AddMinutes(refreshTokenValidityTime);

            await _userManager.UpdateAsync(user);

            string newAccessTokenAsString = new JwtSecurityTokenHandler().WriteToken(newAccessToken);

            return new RefreshTokenResponse()
            {
                Message = "Tokens refreshed Successfuly",
                AccessToken = newAccessTokenAsString,
                RefreshToken = newRefreshToken,
                IsSuccess = true,
            };
        }
    }
}
