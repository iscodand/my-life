using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Responses;
using Profile = MyLifeApp.Domain.Entities.Profile;
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
            string inexistentProfile = "inexistentProfileUsername";

            DetailProfileResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileService.GetProfileByUsernameAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileByUsername(inexistentProfile);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
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
            string inexistentProfile = "inexistentProfileUsername";

            GetFollowingsResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileService.GetProfileFollowingsAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowings(inexistentProfile);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
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
            string inexistentProfile = "inexistentProfileUsername";

            GetFollowingsResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileService.GetProfileFollowersAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetProfileFollowers(inexistentProfile);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

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

        [Fact]
        public async Task FollowProfile_InexistentProfile_ReturnsNotFound()
        {
            // Arrange
            string inexistentProfile = "inexistentProfileUsername";

            BaseResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileService.FollowProfileAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.FollowProfile(inexistentProfile);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        // TO-DO
        // => refactor all controllers and fix status code of them

        [Fact]
        public async Task FollowProfile_AlreadyFollowing_ReturnsBadRequest()
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
                Message = "You already follow this profile.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _profileService.FollowProfileAsync(anotherProfile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.FollowProfile(anotherProfile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UnfollowProfile_ExistentProfile_ReturnsOk()
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
                Message = $"Successfuly Unfollow {anotherProfile.User?.UserName}",
                IsSuccess = true
            };

            A.CallTo(() => _profileService.UnfollowProfileAsync(anotherProfile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnfollowProfile(anotherProfile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UnfollowProfile_InexistentProfile_ReturnsNotFound()
        {
            string inexistentProfile = "inexistentProfileUsername";

            BaseResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileService.UnfollowProfileAsync(inexistentProfile))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnfollowProfile(inexistentProfile);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        // // TO-DO
        // // => refactor all controllers and fix status code of them
        [Fact]
        public async Task UnfollowProfile_NotFollowingProfile_ReturnsBadRequest()
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
                Message = "You not follow this profile.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _profileService.UnfollowProfileAsync(anotherProfile.User!.UserName!))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnfollowProfile(anotherProfile.User!.UserName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }
    }
}
