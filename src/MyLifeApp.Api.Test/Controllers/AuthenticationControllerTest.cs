using MyLifeApp.Application.Interfaces.Services;
using Identity.Infrastructure.Interfaces.Services;
using Identity.Infrastructure.DTOs.Request;
using Identity.Infrastructure.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace MyLifeApp.Api.Test.Controllers
{
    public class AuthenticationControllerTest
    {

        private readonly AuthenticationController _controller;
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;

        public AuthenticationControllerTest()
        {
            _userService = A.Fake<IUserService>();
            _profileService = A.Fake<IProfileService>();
            _controller = new AuthenticationController(_userService, _profileService);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            // Arrange
            RegisterUserRequest request = new()
            {
                Name = "John Doe",
                Username = "johndoe",
                Email = "johndoe@example.com",
                Password = "Testing123!",
                PasswordConfirm = "Testing123!"
            };

            RegisterUserResponse response = new()
            {
                Id = "9599c94a-01c9-4ba7-997b-f4ccfd85d68b",
                IsSuccess = true,
                Message = "User successfully created"
            };

            A.CallTo(() => _userService.RegisterAsync(A<RegisterUserRequest>.Ignored))
                .Returns(Task.FromResult(response));
            A.CallTo(() => _profileService.CreateProfileAsync(A<string>.Ignored))
                .Returns(response.IsSuccess.Equals(true));

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Register_InvalidUser_ReturnsBadRequestResult()
        {
            // Arrange
            RegisterUserRequest request = new()
            {
                Name = "John Doe",
                Username = "johndoe",
                Email = "johndoe@example.com",
                Password = "Testing123!",
                PasswordConfirm = "Testing123!"
            };

            RegisterUserResponse response = new()
            {
                Message = "Error while creating user",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.RegisterAsync(A<RegisterUserRequest>.Ignored))
                .Returns(Task.FromResult(response));
            A.CallTo(() => _profileService.CreateProfileAsync(A<string>.Ignored))
                .Returns(Task.FromResult(response.IsSuccess.Equals(false)));

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            LoginUserRequest request = new()
            {
                Username = "johndoe",
                Password = "password"
            };

            LoginUserResponse response = new()
            {
                Message = "Login Successfuly",
                AccessToken = "testing",
                RefreshToken = "testing",
                IsSuccess = true
            };

            A.CallTo(() => _userService.LoginAsync(A<LoginUserRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequestResult()
        {
            // Arrange
            LoginUserRequest request = new()
            {
                Username = "johndoe",
                Password = "invalidPassword"
            };

            LoginUserResponse response = new()
            {
                Message = "Password invalid. Verify and try again.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.LoginAsync(A<LoginUserRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ReturnsOkResult()
        {
            // Arrange
            RefreshTokenRequest request = new()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            };

            RefreshTokenResponse response = new()
            {
                Message = "Tokens refreshed Successfuly",
                AccessToken = "newAccessToken",
                RefreshToken = "newRefreshToken",
                IsSuccess = true,
            };

            A.CallTo(() => _userService.RefreshTokenAsync(A<RefreshTokenRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.RefreshToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsBadRequestResult()
        {
            // Arrange
            RefreshTokenRequest request = new()
            {
                AccessToken = "invalidAccessToken",
                RefreshToken = "invalidRefreshToken"
            };

            RefreshTokenResponse response = new()
            {
                Message = "Invalid access/refresh token.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.RefreshTokenAsync(A<RefreshTokenRequest>.Ignored))
                .Returns(Task.FromResult(response));
            
            // Act
            var result = await _controller.RefreshToken(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }
    }
}
