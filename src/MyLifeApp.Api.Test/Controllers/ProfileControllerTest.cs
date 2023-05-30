using Xunit;
using FluentAssertions;
using FakeItEasy;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Domain.Entities;
using MyLife.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Identity.Infrastructure.Models;
using MyLifeApp.Application.Dtos.Requests.Profile;

namespace MyLifeApp.Api.Test.Controllers
{
    public class ProfileControllerTest
    {
        private readonly IProfileService _profileService;
        private readonly ProfileController _controller;

        public ProfileControllerTest()
        {
            _profileService = A.Fake<IProfileService>();
            _controller = new ProfileController(_profileService);
        }

        // => verify how implement this unit test
        // [Fact]
        // public Task GetAuthenticatedProfile_ReturnsOk()
        // {
        // }

        [Fact]
        public async Task GetProfileByUsername_ExistentProfile_ReturnsOk()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            DetailProfileResponse response = new()
            {
                Id = profile!.Id,
                Name = profile.User?.Name,
                Username = profile.User?.UserName,
                Bio = profile.Bio,
                BirthDate = profile.BirthDate,
                IsPrivate = profile.IsPrivate,
                Message = "Success",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.GetProfileByUsernameAsync(profile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileByUsername(profile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetProfileByUsername_InexistentProfile_ReturnsNotFound()
        {
            // Arrange
            string inexistentUsername = "inexistent";

            DetailProfileResponse response = new()
            {
                IsSuccess = false
            };

            A.CallTo(() => _profileService.GetProfileByUsernameAsync(inexistentUsername))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileByUsername(inexistentUsername);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProfile_ExistentProfile_ReturnsOk()
        {
            // Arrange
            UpdateProfileRequest request = new()
            {
                Bio = "new bio! yay",
                Location = "IDK my location",
                BirthDate = new DateTime(),
                IsPrivate = false
            };

            BaseResponse response = new()
            {
                Message = "Profile Successfuly Updated",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.UpdateProfileAsync(A<UpdateProfileRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdateProfile(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetProfileFollowings_ExistentProfile_ReturnsOk()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var followings = A.Fake<ICollection<GetProfileResponse>>();

            GetFollowingsResponse response = new()
            {
                Profiles = followings,
                Message = "Success",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.GetProfileFollowingsAsync(profile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowings(profile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetProfileFollowings_InexistentProfile_ReturnsNotFound()
        {
            // Arrange
            string inexistentProfile = "inexistentProfile";

            GetFollowingsResponse response = new()
            {
                IsSuccess = false
            };

            A.CallTo(() => _profileService.GetProfileFollowingsAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowings(inexistentProfile);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProfileFollowers_ExistentProfile_ReturnsOk()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var followers = A.Fake<ICollection<GetProfileResponse>>();

            GetFollowingsResponse response = new()
            {
                Profiles = followers,
                Message = "Success",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.GetProfileFollowersAsync(profile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowers(profile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetProfileFollowers_InexistentProfile_ReturnsNotFound()
        {
            // Arrange
            string inexistentProfile = "inexistentProfile";

            GetFollowingsResponse response = new()
            {
                IsSuccess = false
            };

            A.CallTo(() => _profileService.GetProfileFollowersAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowers(inexistentProfile);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // TO-DO
        // => refactor all controllers and fix status code of them

        [Fact]
        public async Task FollowProfile_ExistentProfile_ReturnsOk()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var anotherUser = A.Fake<User>();
            anotherProfile.User = anotherUser;

            BaseResponse response = new()
            {
                Message = $"Now you follow {anotherProfile.User?.UserName}",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.FollowProfileAsync(anotherProfile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.FollowProfile(anotherProfile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }
    }
}
