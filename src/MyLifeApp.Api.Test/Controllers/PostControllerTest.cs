using FakeItEasy;
using Xunit;
using FluentAssertions;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Interfaces.Services;
using MyLife.Api.Controllers;
using MyLifeApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Api.Test.Controllers
{
    public class PostControllerTest
    {
        private readonly IPostService _postService;
        private readonly PostController _controller;

        public PostControllerTest()
        {
            _postService = A.Fake<IPostService>();
            _controller = new PostController(_postService);
        }

        [Fact]
        public async Task GetPublicPosts_ReturnsOkResult()
        {
            // Arrange
            var postsList = A.Fake<List<GetPostsResponse>>();

            GetAllPostsResponse response = new()
            {
                Posts = postsList,
                Message = "Success",
                IsSuccess = true
            };

            A.CallTo(() => _postService.GetPublicPostsAsync()).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetPublicPosts();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetPostById_ExistentPost_ReturnsOkResult()
        {
            // Arrange
            var post = A.Fake<Post>();

            var profileDto = A.Fake<GetProfileResponse>();

            DetailPostResponse response = new()
            {
                Title = "Testing",
                Description = "Description of Test",
                Profile = profileDto,
                Message = "Success",
                IsSuccess = true
            };

            A.CallTo(() => _postService.GetPostByIdAsync(A<Guid>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetPost(post.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetPostById_InexistentPost_ReturnsNotFound()
        {
            // Arrange
            var inexistentId = Guid.NewGuid();

            DetailPostResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.GetPostByIdAsync(A<Guid>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetPost(inexistentId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreatePost_ValidPost_ReturnsCreated()
        {
            // Arrange
            CreatePostRequest request = new()
            {
                Title = "Testing",
                Description = "Description of Test",
                IsPrivate = false,
                Tags = null
            };

            BaseResponse response = new()
            {
                Message = "Post successfuly created",
                IsSuccess = true
            };

            A.CallTo(() => _postService.CreatePostAsync(A<CreatePostRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task CreatePost_InvalidPost_ReturnsBadRequest()
        {
            // Arrange
            CreatePostRequest request = new()
            {
                Title = "",
                Description = "!@#a",
                IsPrivate = false,
                Tags = null
            };

            BaseResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postService.CreatePostAsync(A<CreatePostRequest>.Ignored)).
                Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdatePost_ValidPost_ReturnsBadRequest()
        {
            // Arrange
            var post = A.Fake<Post>();

            UpdatePostRequest request = new()
            {
                Title = "New title",
                Description = "New Description Testing",
                IsPrivate = false
            };

            BaseResponse response = new()
            {
                Message = "Post Successfuly Updated",
                IsSuccess = true
            };

            A.CallTo(() => _postService.UpdatePostAsync(post.Id, A<UpdatePostRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Update(post.Id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdatePost_InexistentPost_ReturnsBadRequest()
        {
            // Arrange
            var inexistentId = Guid.NewGuid();

            UpdatePostRequest request = new()
            {
                Title = "New title",
                Description = "New Description Testing",
                IsPrivate = false
            };

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.UpdatePostAsync(inexistentId, A<UpdatePostRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Update(inexistentId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeletePost_ExistentPost_ReturnsNoContent()
        {
            // Arrange
            var post = A.Fake<Post>();

            BaseResponse response = new()
            {
                IsSuccess = true
            };

            A.CallTo(() => _postService.DeletePostAsync(post.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Delete(post.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePost_InexistentPost_ReturnsNoContent()
        {
            // Arrange
            var inexistentId = Guid.NewGuid();

            BaseResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.DeletePostAsync(inexistentId)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Delete(inexistentId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task LikePost_ExistentPost_ReturnsOk()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();

            BaseResponse response = new()
            {
                Message = "Post successfuly liked.",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postService.LikePostAsync(post.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.LikePost(post.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task LikePost_InexistentPost_ReturnsNotFound()
        {
            // Arrange
            Guid inexistentPost = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found.",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.LikePostAsync(inexistentPost)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.LikePost(inexistentPost);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task LikePost_AlreadyLikedByProfile_ReturnsBadRequest()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();

            BaseResponse response = new()
            {
                Message = "You already liked this post.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postService.LikePostAsync(post.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.LikePost(post.Id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UnlikePost_ExistentPost_ReturnsOk()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();

            BaseResponse response = new()
            {
                Message = "Post successfuly unliked.",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postService.UnlikePostAsync(post.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnlikePost(post.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UnlikePost_InexistentPost_ReturnsNotFound()
        {
            // Arrange
            Guid inexistentPost = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found.",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.UnlikePostAsync(inexistentPost)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnlikePost(inexistentPost);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }


        [Fact]
        public async Task UnlikePost_NotLikedByProfile_ReturnsBadRequest()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();

            BaseResponse response = new()
            {
                Message = "You doens't like this post yet.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postService.UnlikePostAsync(post.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnlikePost(post.Id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }
    }
}
