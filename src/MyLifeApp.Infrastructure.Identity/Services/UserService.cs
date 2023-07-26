﻿using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.DTOs.Response;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyLifeApp.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public UserService(ITokenService tokenService,
            UserManager<User> userManager,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IAuthenticatedUserService authenticatedUserService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _configuration = configuration;
            _httpContext = httpContext;
            _authenticatedUserService = authenticatedUserService;
        }

        public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest userRequest)
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
                return new RegisterUserResponse()
                {
                    Message = "Error while creating user",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description),
                    StatusCode = 400
                };
            }

            return new RegisterUserResponse()
            {
                Id = user.Id,
                Message = "User successfuly created",
                IsSuccess = true,
                StatusCode = 201
            };
        }

        public async Task<LoginUserResponse> LoginAsync(LoginUserRequest userRequest)
        {
            User? user = await _userManager.FindByNameAsync(userRequest.Username);

            if (user == null)
            {
                return new LoginUserResponse()
                {
                    Message = "Username doens't exists. Verify and try again.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            bool verifyPassword = await _userManager.CheckPasswordAsync(user, userRequest.Password);

            if (!verifyPassword)
            {
                return new LoginUserResponse()
                {
                    Message = "Password invalid. Verify and try again.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Generating tokens
            JwtSecurityToken accessToken = _tokenService.GenerateAccessToken(claims);
            string refreshToken = _tokenService.GenerateRefreshToken();

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
                IsSuccess = true,
                StatusCode = 200
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest tokenRequest)
        {
            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            ClaimsPrincipal principal = _tokenService.GetPrincipalForExpiredToken(accessToken);

            if (principal == null)
            {
                return new RefreshTokenResponse()
                {
                    Message = "Invalid access/refresh token.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            User? user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenValidityTime <= DateTime.Now)
            {
                return new RefreshTokenResponse()
                {
                    Message = "Invalid access/refresh token",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            JwtSecurityToken newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());
            string newRefreshToken = _tokenService.GenerateRefreshToken();

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
                StatusCode = 200
            };
        }

        // todo => pay attention to refactor (see how ChangePasswordAsync from UserManager works!)
        public async Task<BaseResponse> UpdatePasswordAsync(UpdatePasswordRequest request)
        {
            User? user = await _authenticatedUserService.GetAuthenticatedUserAsync();
            bool checkOldPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword!);

            if (!checkOldPassword)
            {
                return new BaseResponse()
                {
                    Message = "Incorrect old password",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            if (request.OldPassword == request.NewPassword)
            {
                return new BaseResponse()
                {
                    Message = "New password can't be equal to old password",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new BaseResponse()
                {
                    Message = "New passwords don't match",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            await _userManager.ChangePasswordAsync(user, request.OldPassword!, request.NewPassword!);

            return new BaseResponse()
            {
                Message = "Password successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };
        }
    }
}
