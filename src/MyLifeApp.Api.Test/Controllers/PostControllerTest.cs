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

            A.CallTo(() => _postService.GetPostByIdAsync(A<Guid>.Ignored));

            // Act
            var result = await _controller.GetPost(inexistentId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
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
                IsSuccess = false
            };

            A.CallTo(() => _postService.CreatePostAsync(A<CreatePostRequest>.Ignored)).
                Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
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
                IsSuccess = false
            };

            A.CallTo(() => _postService.UpdatePostAsync(inexistentId, A<UpdatePostRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Update(inexistentId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                   .Which.Value.Should().BeEquivalentTo(response);
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
                IsSuccess = false
            };

            A.CallTo(() => _postService.DeletePostAsync(inexistentId)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.Delete(inexistentId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
