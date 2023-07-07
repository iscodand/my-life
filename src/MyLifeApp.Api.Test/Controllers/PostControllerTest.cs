using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Interfaces.Services;
using MyLife.Api.Controllers;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Api.Test.Controllers
{
    public class PostControllerTest
    {
        private readonly IPostService _postService;
        private readonly ICacheService _cacheService;
        private readonly PostController _controller;

        public PostControllerTest()
        {
            _postService = A.Fake<IPostService>();
            _cacheService = A.Fake<ICacheService>();
            _controller = new PostController(_postService, _cacheService);
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

            A.CallTo(() => _postService.GetPostByIdAsync(A<int>.Ignored))
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
            DetailPostResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.GetPostByIdAsync(A<int>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.GetPost(101);

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
            var result = await _controller.CreatePost(request);

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
            var result = await _controller.CreatePost(request);

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
            var result = await _controller.UpdatePost(post.Id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdatePost_InexistentPost_ReturnsBadRequest()
        {
            // Arrange
            var inexistentId = Guid.NewGuid().ToString();

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

            A.CallTo(() => _postService.UpdatePostAsync(A<int>.Ignored, A<UpdatePostRequest>.Ignored))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdatePost(101, request);

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
            var result = await _controller.DeletePost(post.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePost_InexistentPost_ReturnsNoContent()
        {
            // Arrange
            var inexistentId = Guid.NewGuid().ToString();

            BaseResponse response = new()
            {
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.DeletePostAsync(A<int>.Ignored)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.DeletePost(101);

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
            BaseResponse response = new()
            {
                Message = "Post not found.",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.LikePostAsync(A<int>.Ignored)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.LikePost(101);

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
            BaseResponse response = new()
            {
                Message = "Post not found.",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.UnlikePostAsync(A<int>.Ignored)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UnlikePost(101);

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

        [Fact]
        public async Task CommentPost_ValidCommentAndPost_ReturnsCreated()
        {
            // Arrange
            var post = A.Fake<Post>();

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            BaseResponse response = new()
            {
                Message = "Comment successfuly added",
                IsSuccess = true,
                StatusCode = 201
            };

            A.CallTo(() => _postService.CommentPostAsync(post.Id, request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.CommentPost(post.Id, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CommentPost_InexistentPost_ReturnsNotFound()
        {
            // Arrange
            var post = A.Fake<Post>();

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.CommentPostAsync(post.Id, request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.CommentPost(post.Id, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateComment_ExistentAndValidComment_ReturnsSuccess()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            BaseResponse response = new()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postService.UpdateCommentAsync(comment.Id, request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdateComment(comment.Id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateComment_InexistentComment_ReturnsNotFound()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            BaseResponse response = new()
            {
                Message = "Comment not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.UpdateCommentAsync(comment.Id, request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdateComment(comment.Id, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateComment_InvalidUpdate_ReturnsBadRequest()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            BaseResponse response = new()
            {
                Message = "Only comment author can update the comment",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postService.UpdateCommentAsync(comment.Id, request)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.UpdateComment(comment.Id, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task DeleteComment_ValidComment_ReturnsOk()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            BaseResponse response = new()
            {
                IsSuccess = true,
                StatusCode = 204
            };

            A.CallTo(() => _postService.DeleteCommentAsync(comment.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.DeleteComment(comment.Id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteComment_InexistentComment_ReturnsNotFound()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            BaseResponse response = new()
            {
                Message = "Comment not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postService.DeleteCommentAsync(comment.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.DeleteComment(comment.Id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteComment_InvalidDelete_ReturnsBadRequest()
        {
            // Arrange
            var comment = A.Fake<PostComment>();

            BaseResponse response = new()
            {
                Message = "Only Comment Author or Post Author can delete the comment",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postService.DeleteCommentAsync(comment.Id)).Returns(Task.FromResult(response));

            // Act
            var result = await _controller.DeleteComment(comment.Id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }
    }
}