using Identity.Infrastructure.DTOs.Request;
using Identity.Infrastructure.DTOs.Response;
using Identity.Infrastructure.Interfaces;
using MyLifeApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using Xunit;
using FluentAssertions;
using Identity.Infrastructure.Models;

namespace MyLifeApp.Api.Test.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly AuthenticationController _controller;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IProfileRepository _profileRepositoryMock;
        private readonly ITokenRepository _tokenRepositoryMock;

        public AuthenticationControllerTests()
        {
            _userRepositoryMock = A.Fake<IUserRepository>();
            _profileRepositoryMock = A.Fake<IProfileRepository>();
            _tokenRepositoryMock = A.Fake<ITokenRepository>();
            _controller = new AuthenticationController(_userRepositoryMock, _profileRepositoryMock);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Username = "johndoe",
                Email = "johndoe@example.com",
                Password = "Testing123!",
                PasswordConfirm = "Testing123!"
            };

            var response = new RegisterUserResponse
            {
                Id = "9599c94a-01c9-4ba7-997b-f4ccfd85d68b",
                IsSuccess = true,
                Message = "User successfully created"
            };

            A.CallTo(() => _userRepositoryMock.Register(A<RegisterUserRequest>.Ignored))
                .Returns(Task.FromResult(response));
            A.CallTo(() => _profileRepositoryMock.RegisterProfile(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Register_InvalidUser_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Username = "     ",
                Email = "johndoe@example.com",
                Password = "Testing123!",
                PasswordConfirm = "Testing123!"
            };

            var errors = new List<string> { "Username is required." };

            var response = new RegisterUserResponse
            {
                Message = "Error while creating user",
                IsSuccess = false,
                Errors = errors
            };


            A.CallTo(() => _userRepositoryMock.Register(A<RegisterUserRequest>.Ignored))
                .Returns(Task.FromResult(response));
            A.CallTo(() => _profileRepositoryMock.RegisterProfile(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_FailedUserCreation_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Username = "johndoe",
                Email = "johndoe@example.com",
                Password = "Testing123!",
                PasswordConfirm = "Testing123!"
            };

            var errors = new List<string> { "Invalid email format", "Password is too weak" };

            var response = new RegisterUserResponse
            {
                IsSuccess = false,
                Message = "Error while creating user",
                Errors = errors
            };

            A.CallTo(() => _userRepositoryMock.Register(A<RegisterUserRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "johndoe",
                Password = "password"
            };

            var user = new User
            {
                Id = "9599c94a-01c9-4ba7-997b-f4ccfd85d68b",
                UserName = "johndoe"
            };

            var response = new LoginUserResponse
            {
                IsSuccess = true,
                Message = "Login Successfully",
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            A.CallTo(() => _userRepositoryMock.Login(A<LoginUserRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // TODO => fix the return of access token response mock
            //A.CallTo(() => _tokenRepositoryMock.GenerateAccessToken(A<List<Claim>>.Ignored))
            //    .Returns(response["AccessToken"];

            A.CallTo(() => _tokenRepositoryMock.GenerateRefreshToken())
                .Returns("refresh-token");

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Login_InvalidUsername_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "nonexistentuser",
                Password = "password"
            };

            var response = new LoginUserResponse
            {
                IsSuccess = false,
                Message = "Username doesn't exist. Verify and try again."
            };

            A.CallTo(() => _userRepositoryMock.Login(A<LoginUserRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "johndoe",
                Password = "12345678"
            };

            var user = new User
            {
                Id = "9599c94a-01c9-4ba7-997b-f4ccfd85d68b",
                UserName = "johndoe",
            };

            var response = new LoginUserResponse
            {
                Message = "Password invalid. Verify and try again.",
                IsSuccess = false
            };

            A.CallTo(() => _userRepositoryMock.Login(request))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task RefreshToken_ValidRequest_Returns200()
        {
            // Arrange
            var response = new RefreshTokenResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                IsSuccess = true
            };

            A.CallTo(() => _userRepositoryMock.RefreshToken(A<RefreshTokenRequest>.Ignored))
                .Returns(response);

            // Act
            var result = await _controller.RefreshToken(new RefreshTokenRequest());

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task RefreshToken_InvalidRequest_Returns400()
        {
            // Arrange

            var refreshTokenResponse = new RefreshTokenResponse
            {
                Message = "",
                IsSuccess = false
            };

            A.CallTo(() => _userRepositoryMock.RefreshToken(A<RefreshTokenRequest>._))
                .Returns(refreshTokenResponse);

            // Act
            var result = await _controller.RefreshToken(new RefreshTokenRequest());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(400);
            result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(refreshTokenResponse);
        }

        [Fact]
        public async Task RefreshToken_InvalidModelState_Returns500()
        {
            // Arrange
            _controller.ModelState.AddModelError("key", "error");

            // Act
            var result = await _controller.RefreshToken(new RefreshTokenRequest());

            // Assert
            result.Should().BeOfType<StatusCodeResult>()
                .Which.StatusCode.Should().Be(500);
        }
    }
}
