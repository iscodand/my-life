using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.Models;
using MyLifeApp.Infrastructure.Identity.Services;
using MyLifeApp.Infrastructure.Shared.Services.Email;

namespace MyLifeApp.WebApi.Test.Services
{
    public class UserServiceTest
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly IEmailService _mailService;

        public UserServiceTest()
        {
            _tokenService = A.Fake<ITokenService>();
            _userManager = A.Fake<UserManager<User>>();
            _configuration = A.Fake<IConfiguration>();
            _httpContext = A.Fake<IHttpContextAccessor>();
            _authenticatedUserService = A.Fake<IAuthenticatedUserService>();
            _mailService = A.Fake<IEmailService>();
            _authenticationService = new AuthenticationService(_tokenService, _userManager, _httpContext, _configuration, _authenticatedUserService, _mailService);
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
            var result = await _authenticationService.UpdatePasswordAsync(request);

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
            var result = await _authenticationService.UpdatePasswordAsync(request);

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
            var result = await _authenticationService.UpdatePasswordAsync(request);

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
            var result = await _authenticationService.UpdatePasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ForgetPasswordAsync_ValidEmail_ReturnsOk()
        {
            // Arrange
            User user = A.Fake<User>();
            string fakeToken = "token";

            ForgetPasswordRequest request = new()
            {
                Email = user.Email
            };

            BaseResponse response = new()
            {
                Message = "Check your e-mail and follow instructions to recover your password",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _userManager.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user)).Returns(Task.FromResult(fakeToken));

            // Act
            var result = await _authenticationService.ForgetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ForgetPasswordAsync_InexistentEmail_ReturnsBadRequest()
        {
            // Arrange
            string inexistentEmail = "inexistent@email.com";
            User inexistentUser = null;

            ForgetPasswordRequest request = new()
            {
                Email = inexistentEmail
            };

            BaseResponse response = new()
            {
                Message = "E-mail not found",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userManager.FindByEmailAsync(inexistentEmail)).Returns(Task.FromResult(inexistentUser));

            // Act
            var result = await _authenticationService.ForgetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ResetPasswordAsync_ValidRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            User user = A.Fake<User>();

            var request = new ResetPasswordRequest
            {
                Email = user.Email,
                Token = "valid_token",
                NewPassword = "newPassword123",
                ConfirmNewPassword = "newPassword123"
            };

            BaseResponse response = new()
            {
                Message = "Password Successfuly Reseted",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _userManager.FindByEmailAsync(request.Email!)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.NewPassword)).Returns(false);
            A.CallTo(() => _userManager.ResetPasswordAsync(user, A<string>._, request.NewPassword)).Returns(IdentityResult.Success);

            // Act
            var result = await _authenticationService.ResetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ResetPasswordAsync_InexistentEmail_ReturnsBadRequest()
        {
            // Arrange
            string fakeEmail = "teste@email.com";
            string token = "fakeToken";
            User user = null;

            ResetPasswordRequest request = new()
            {
                Email = fakeEmail,
                Token = token,
                NewPassword = "NewPass123!",
                ConfirmNewPassword = "NewPass123!"
            };

            BaseResponse response = new()
            {
                Message = "E-mail not found",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userManager.FindByEmailAsync(fakeEmail)).Returns(Task.FromResult(user));

            // Act
            var result = await _authenticationService.ResetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ResetPasswordAsync_DifferentPasswords_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();
            string token = "fakeToken";

            ResetPasswordRequest request = new()
            {
                Email = user.Email,
                Token = token,
                NewPassword = "NewPass123!",
                ConfirmNewPassword = "DifferentPass123!"
            };

            BaseResponse response = new()
            {
                Message = "New passwords don't match",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userManager.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));

            // Act
            var result = await _authenticationService.ResetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        // todo => verify how implement this unit test (yeah, I know how to implement this validation)
        [Fact]
        public async Task ResetPasswordAsync_NewPasswordEqualsToOldPassword_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();
            string token = "fakeToken";

            ResetPasswordRequest request = new()
            {
                Email = user.Email,
                Token = token,
                NewPassword = "OldPass123!",
                ConfirmNewPassword = "OldPass123!"
            };

            BaseResponse response = new()
            {
                Message = "New password can't be equal to old password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userManager.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.NewPassword))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _authenticationService.ResetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            User user = A.Fake<User>();
            string invalidToken = "invalidToken";

            ResetPasswordRequest request = new()
            {
                Email = user.Email,
                Token = invalidToken,
                NewPassword = "NewPass123!",
                ConfirmNewPassword = "NewPass123!"
            };

            BaseResponse response = new()
            {
                Message = "Ops! Invalid token. Send a new email for reset your password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userManager.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.ResetPasswordAsync(user, invalidToken, request.NewPassword))
                .Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var result = await _authenticationService.ResetPasswordAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }
    }
}