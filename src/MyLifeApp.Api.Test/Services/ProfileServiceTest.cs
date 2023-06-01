using Xunit;
using FakeItEasy;
using FluentAssertions;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Identity.Infrastructure.Models;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;

namespace MyLifeApp.Api.Test.Services
{
    public class ProfileServiceTest
    {
        private readonly ProfileService _profileService;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<User> _manager;

        public ProfileServiceTest()
        {
            _profileRepository = A.Fake<IProfileRepository>();
            _mapper = A.Fake<IMapper>();
            _context = A.Fake<IHttpContextAccessor>();
            _manager = A.Fake<UserManager<User>>();
            _profileService = new ProfileService(_profileRepository, _mapper, _context, _manager);
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

        // // TO-DO => implement this when I know how to implement
        // [Fact]
        // public async Task GetAuthenticatedProfile_SuccessfulyAuthenticated_ReturnsOk()
        // {
        // }

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
        }

        // // TODO => verify how test Update Profile
        // [Fact]
        // public async Task UpdateProfileAsync_ValidUpdate_ReturnsSuccess()
        // {
        //     // Arrange
        //     var profile = A.Fake<Profile>();
        //     var user = A.Fake<User>();
        //     profile.User = user;

        //     UpdateProfileRequest request = new()
        //     {
        //         Bio = "Testing bio",
        //         Location = "Testing location",
        //         BirthDate = DateTime.Now,
        //         IsPrivate = false
        //     };

        //     BaseResponse response = new()
        //     {
        //         Message = "Profile successfuly updated",
        //         IsSuccess = true,
        //         StatusCode = 200
        //     };

        //     A.CallTo(() => _mapper.Map(request, profile)).Returns(profile);
        //     A.CallTo(() => _profileRepository.UpdateAsync(profile));

        //     // Act
        //     var result = await _profileService.UpdateProfileAsync(request);

        //     // Assert
        //     result.Should().BeEquivalentTo(response);
        //     A.CallTo(() => _profileRepository.SaveAsync()).MustHaveHappenedOnceExactly();
        // }

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
        }

        // TODO => verify how get authenticated profile in unit tests
    }
}