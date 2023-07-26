﻿using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.WebApi.Controllers;

namespace MyLifeApp.WebApi.Test.Controllers
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

        [Fact]
        public async Task UpdatePassword_ValidPasswords_ReturnsOk()
        {
            // Arrange
            UpdatePasswordRequest request = new()
            {
                OldPassword = "Test123!",
                NewPassword = "Test321!",
                ConfirmNewPassword = "Test321!"
            };

            BaseResponse response = new()
            {
                Message = "Password successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _userService.UpdatePasswordAsync(request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdatePassword(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdatePassword_IncorrectOldPassword_ReturnsBadRequest()
        {
            // Arrange
            UpdatePasswordRequest request = new()
            {
                OldPassword = "Incorrect123!",
                NewPassword = "NewPass123!",
                ConfirmNewPassword = "NewPass123!",
            };

            BaseResponse response = new()
            {
                Message = "Incorrect old password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.UpdatePasswordAsync(request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdatePassword(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdatePassword_NewPasswordsDontMatch_ReturnsBadRequest()
        {
            // Arrange
            UpdatePasswordRequest request = new()
            {
                OldPassword = "Test123!",
                NewPassword = "NewPass123!",
                ConfirmNewPassword = "Incorrect123!",
            };

            BaseResponse response = new()
            {
                Message = "New passwords don't match",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.UpdatePasswordAsync(request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdatePassword(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdatePassword_NewPasswordEqualsOldPassword_ReturnsBadRequest()
        {
            // Arrange
            UpdatePasswordRequest request = new()
            {
                OldPassword = "Test123!",
                NewPassword = "Test123!",
                ConfirmNewPassword = "Test123!",
            };

            BaseResponse response = new()
            {
                Message = "New password can't be equal to old password",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _userService.UpdatePasswordAsync(request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdatePassword(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }
    }
}