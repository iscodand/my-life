using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.Models;
using MyLifeApp.Infrastructure.Identity.Services;

namespace MyLifeApp.WebApi.Test.Services
{
    public class UserServiceTest
    {
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public UserServiceTest()
        {
            _tokenService = A.Fake<ITokenService>();
            _userManager = A.Fake<UserManager<User>>();
            _configuration = A.Fake<IConfiguration>();
            _httpContext = A.Fake<IHttpContextAccessor>();
            _authenticatedUserService = A.Fake<IAuthenticatedUserService>();
            _userService = new UserService(_tokenService, _userManager, _httpContext, _configuration, _authenticatedUserService);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ValidPasswords_ReturnsSuccess()
        {
            // Arrange
            User user = A.Fake<User>();

            UpdatePasswordRequest request = new()
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            BaseResponse response = new()
            {
                Message = "Password successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _authenticatedUserService.GetAuthenticatedUserAsync()).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.OldPassword))
                .Returns(Task.FromResult(true));
            A.CallTo(() => _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _userService.UpdatePasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePasswordAsync_InvalidOldPassword_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();

            UpdatePasswordRequest request = new()
            {
                OldPassword = "InvalidOldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            BaseResponse response = new()
            {
                Message = "Incorrect old password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _authenticatedUserService.GetAuthenticatedUserAsync()).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.NewPassword))
                .Returns(Task.FromResult(false));

            // Act
            var result = await _userService.UpdatePasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePasswordAsync_DifferentNewPasswords_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();

            UpdatePasswordRequest request = new()
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewDifferentPassword123!",
            };

            BaseResponse response = new()
            {
                Message = "New passwords don't match",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _authenticatedUserService.GetAuthenticatedUserAsync()).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.OldPassword))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _userService.UpdatePasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePasswordAsync_NewPasswordEqualsOldPassword_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();

            UpdatePasswordRequest request = new()
            {
                OldPassword = "OldPassword123!",
                NewPassword = "OldPassword123!",
                ConfirmNewPassword = "OldPassword123!",
            };

            BaseResponse response = new()
            {
                Message = "New password can't be equal to old password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _authenticatedUserService.GetAuthenticatedUserAsync()).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.OldPassword))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _userService.UpdatePasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }
    }
}