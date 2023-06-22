using Identity.Infrastructure.Models;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Services;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Api.Test.Services
{
    public class ProfileServiceTest
    {
        private readonly ProfileService _profileService;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public ProfileServiceTest()
        {
            _profileRepository = A.Fake<IProfileRepository>();
            _mapper = A.Fake<IMapper>();
            _authenticatedProfileService = A.Fake<IAuthenticatedProfileService>();
            _profileService = new ProfileService(_profileRepository, _mapper, _authenticatedProfileService);
        }

        [Fact]
        public async Task CreateProfileAsync_SuccessfulyCreate_ReturnsTrue()
        {
            // Arrange
            var user = A.Fake<User>();
            var profileAnalytics = A.Fake<ProfileAnalytics>();

            Profile profile = new()
            {
                UserId = user.Id,
            };

            A.CallTo(() => _profileRepository.CreateAsync(profile)).Returns(Task.FromResult(profile));

            // Act
            var result = await _profileService.CreateProfileAsync(user.Id);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task GetAuthenticatedProfileAsync_SuccessfulyAuthenticated_ReturnsOk()
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
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));

            // Act
            var result = await _profileService.GetAuthenticatedProfileAsync();

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetProfileByUsernameAsync_GetExistentProfile_ReturnsSuccess()
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
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(profile.User!.UserName!))
                .Returns(Task.FromResult(profile));

            // Act
            var result = await _profileService.GetProfileByUsernameAsync(profile.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetProfileByUsernameAsync_InexistentProfile_ReturnsError()
        {
            // Arrange
            string inexistentUsername = "inexistentUsername";
            Profile? nullProfile = null;

            DetailProfileResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(inexistentUsername)).Returns(nullProfile);

            // Act
            var result = await _profileService.GetProfileByUsernameAsync(inexistentUsername);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProfileAsync_ValidUpdate_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            UpdateProfileRequest request = new()
            {
                Bio = "Testing bio",
                Location = "Testing location",
                BirthDate = DateTime.Now,
                IsPrivate = false
            };

            BaseResponse response = new()
            {
                Message = "Profile successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _mapper.Map(request, profile)).Returns(profile);
            A.CallTo(() => _profileRepository.UpdateAsync(profile));

            // Act
            var result = await _profileService.UpdateProfileAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
            A.CallTo(() => _profileRepository.SaveAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetProfileFollowingsAsync_ExistentProfile_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            ICollection<ProfileFollower> followings = A.Fake<ICollection<ProfileFollower>>();
            ICollection<Profile> profileFollowings = A.Fake<ICollection<Profile>>();
            ICollection<GetProfileResponse> profileFollowingsMapper = A.Fake<ICollection<GetProfileResponse>>();

            profile.User = user;
            profile.ProfileFollowers = followings;

            GetFollowingsResponse response = new()
            {
                Profiles = profileFollowingsMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _profileRepository.GetProfileFollowingsAsync(profile))
                .Returns(Task.FromResult(followings));
            A.CallTo(() => _mapper.Map<ICollection<GetProfileResponse>>(profileFollowings))
                .Returns(profileFollowingsMapper);

            // Act
            var result = await _profileService.GetProfileFollowingsAsync(profile.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetProfileFollowingsAsync_InexistentProfile_ReturnsError()
        {
            // Arrange
            string inexistentUsername = "inexistentUsername";
            Profile? nullProfile = null;

            GetFollowingsResponse response = new()
            {
                Message = "Profile not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(inexistentUsername)).Returns(nullProfile);

            // Act
            var result = await _profileService.GetProfileFollowingsAsync(inexistentUsername);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetProfileFollowersAsync_ExistentProfile_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            ICollection<ProfileFollower> followers = A.Fake<ICollection<ProfileFollower>>();
            ICollection<Profile> profileFollowers = A.Fake<ICollection<Profile>>();
            ICollection<GetProfileResponse> profileFollowersMapper = A.Fake<ICollection<GetProfileResponse>>();

            profile.User = user;
            profile.ProfileFollowers = followers;

            GetFollowingsResponse response = new()
            {
                Profiles = profileFollowersMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _profileRepository.GetProfileFollowersAsync(profile))
                .Returns(Task.FromResult(followers));
            A.CallTo(() => _mapper.Map<ICollection<GetProfileResponse>>(profileFollowers))
                .Returns(profileFollowersMapper);

            // Act
            var result = await _profileService.GetProfileFollowersAsync(profile.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetProfileFollowersAsync_InexistentProfile_ReturnsError()
        {
            // Arrange
            string inexistentUsername = "inexistentUsername";
            Profile? nullProfile = null;

            GetFollowingsResponse response = new()
            {
                Message = "Profile not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(inexistentUsername)).Returns(nullProfile);

            // Act
            var result = await _profileService.GetProfileFollowersAsync(inexistentUsername);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task FollowProfileAsync_ExistentProfile_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var follower = A.Fake<Profile>();
            var followerUser = A.Fake<User>();
            follower.User = followerUser;

            BaseResponse response = new()
            {
                Message = $"Now you follow {follower.User?.UserName}",
                IsSuccess = true,
                StatusCode = 200
            };

            ProfileFollower follow = new()
            {
                Profile = profile,
                Follower = follower
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.AddProfileFollower(follow)).Returns(Task.FromResult(follow));

            // Act
            var result = await _profileService.FollowProfileAsync(follower.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task FollowProfileAsync_InexistentProfile_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            Profile? inexistentFollower = null;
            var inexistentUsername = "inexistentUsername";

            BaseResponse response = new()
            {
                Message = "Profile not found",
                IsSuccess = false,
                StatusCode = 404
            };

            ProfileFollower follow = new()
            {
                Profile = profile,
                Follower = inexistentFollower
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(inexistentUsername)).Returns(inexistentFollower);

            // Act
            var result = await _profileService.FollowProfileAsync(inexistentUsername);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        // TODO => verify how to implement this tests above
        [Fact]
        public async Task FollowProfileAsync_AlreadyFollowingProfile_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var follower = A.Fake<Profile>();
            var followerUser = A.Fake<User>();
            follower.User = followerUser;

            BaseResponse response = new()
            {
                Message = "You already follow this profile.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(follower.User!.UserName!)).Returns(Task.FromResult(follower));
            A.CallTo(() => _profileRepository.ProfileFollowerExistsAsync(profile, follower)).Returns(Task.FromResult(true));

            // Act
            var result = await _profileService.FollowProfileAsync(follower.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UnfollowProfileAsync_ExistentProfile_ReturnSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var follower = A.Fake<Profile>();
            var followerUser = A.Fake<User>();
            var profileFollower = A.Fake<ProfileFollower>();
            follower.User = followerUser;

            BaseResponse response = new()
            {
                Message = $"Successfuly Unfollow {follower.User?.UserName}",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(follower.User!.UserName!)).Returns(Task.FromResult(follower));
            A.CallTo(() => _profileRepository.ProfileFollowerExistsAsync(profile, follower)).Returns(Task.FromResult(true));
            A.CallTo(() => _profileRepository.GetProfileFollowerAsync(profile, follower)).Returns(Task.FromResult(profileFollower));
            A.CallTo(() => _profileRepository.RemoveProfileFollower(profileFollower));

            // Act
            var result = await _profileService.UnfollowProfileAsync(follower.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UnfollowProfileAsync_InexistentProfile_ReturnError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            Profile? inexistentFollower = null;
            var inexistentUsername = "inexistentUsername";

            BaseResponse response = new()
            {
                Message = "Profile not found",
                IsSuccess = false,
                StatusCode = 404
            };

            ProfileFollower follow = new()
            {
                Profile = profile,
                Follower = inexistentFollower
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(inexistentUsername)).Returns(inexistentFollower);

            // Act
            var result = await _profileService.UnfollowProfileAsync(inexistentUsername);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UnfollowProfileAsync_NotFollowing_ReturnError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var follower = A.Fake<Profile>();
            var followerUser = A.Fake<User>();
            var profileFollower = A.Fake<ProfileFollower>();
            follower.User = followerUser;

            BaseResponse response = new()
            {
                Message = "You not follow this profile.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _profileRepository.GetProfileByUsernameAsync(follower.User!.UserName!)).Returns(Task.FromResult(follower));
            A.CallTo(() => _profileRepository.ProfileFollowerExistsAsync(profile, follower)).Returns(Task.FromResult(false));

            // Act
            var result = await _profileService.UnfollowProfileAsync(follower.User!.UserName!);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }
    }
}