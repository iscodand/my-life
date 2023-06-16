using Xunit;
using FakeItEasy;
using FluentAssertions;
using MyLifeApp.Application.Services;
using MyLifeApp.Application.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Identity.Infrastructure.Models;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;

namespace MyLifeApp.Api.Test
{
    public class PostServiceTest
    {
        private readonly IPostService _postService;
        private readonly IPostRepository _postRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<User> _manager;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public PostServiceTest()
        {
            _profileRepository = A.Fake<IProfileRepository>();
            _postRepository = A.Fake<IPostRepository>();
            _mapper = A.Fake<IMapper>();
            _context = A.Fake<IHttpContextAccessor>();
            _manager = A.Fake<UserManager<User>>();
            _authenticatedProfileService = A.Fake<IAuthenticatedProfileService>();
            _postService = new PostService(_postRepository, _profileRepository, _mapper, _context, _manager, _authenticatedProfileService);
        }

        [Fact]
        public async Task GetPublicPostsAsync_GetAllPublicPosts_ReturnSuccess()
        {
            // Arrange
            var posts = A.Fake<ICollection<Post>>();
            var getPostsResponse = A.Fake<ICollection<GetPostsResponse>>();

            GetAllPostsResponse response = new()
            {
                Posts = getPostsResponse,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postRepository.GetPublicPostsAsync()).Returns(Task.FromResult(posts));
            A.CallTo(() => _mapper.Map<ICollection<GetPostsResponse>>(posts)).Returns(getPostsResponse);

            // Act
            var result = await _postService.GetPublicPostsAsync();

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetPostByIdAsync_ExistentPost_ReturnsSuccess()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            profile.User = user;
            post.Profile = profile;

            var postDetail = A.Fake<DetailPostResponse>();
            var profileMapper = A.Fake<GetProfileResponse>();

            DetailPostResponse response = new()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profileMapper,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(true);
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _mapper.Map<DetailPostResponse>(post)).Returns(postDetail);
            A.CallTo(() => _mapper.Map<GetProfileResponse>(profile)).Returns(profileMapper);

            // Act
            var result = await _postService.GetPostByIdAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetPostByIdAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            DetailPostResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(false);

            // Act
            var result = await _postService.GetPostByIdAsync(inexistentPostGuid);

            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task CreatePostAsync_ValidPost_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            CreatePostRequest request = new()
            {
                Title = "Testing",
                Description = "Testing description",
                IsPrivate = false
            };

            BaseResponse response = new()
            {
                Message = "Post successfuly created.",
                IsSuccess = true,
                StatusCode = 201
            };

            var post = A.Fake<Post>();
            post.Profile = profile;

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _mapper.Map<Post>(request)).Returns(post);
            A.CallTo(() => _postRepository.CreateAsync(post)).Returns(Task.FromResult(post));

            // Act
            var result = await _postService.CreatePostAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(201);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePostAsync_ValidAndExistentPost_ReturnsSuccess()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            profile.User = user;
            post.Profile = profile;

            UpdatePostRequest request = new()
            {
                Title = "Testing title",
                Description = "Testing update description",
                IsPrivate = false,
            };

            BaseResponse response = new()
            {
                Message = "Post Successfuly Updated",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetByIdAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _mapper.Map<Post>(request)).Returns(post);
            A.CallTo(() => _postRepository.SaveAsync());

            // Act
            var result = await _postService.UpdatePostAsync(post.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            UpdatePostRequest request = new()
            {
                Title = "Testing title",
                Description = "Testing update description",
                IsPrivate = false,
            };

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UpdatePostAsync(inexistentPostGuid, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePostAsync_UpdateWithNotPostCreator_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var antotherUser = A.Fake<User>();
            anotherProfile.User = antotherUser;

            var post = A.Fake<Post>();
            post.Profile = profile;

            UpdatePostRequest request = new()
            {
                Title = "Testing title",
                Description = "Testing update description",
                IsPrivate = false,
            };

            BaseResponse response = new()
            {
                Message = "Only post creator can update the post.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetByIdAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(anotherProfile));

            // Act
            var result = await _postService.UpdatePostAsync(post.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePostAsync_ExistentAndValidPost_ReturnsSuccess()
        {
            // Arrange
            var post = A.Fake<Post>();
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            profile.User = user;
            post.Profile = profile;

            BaseResponse response = new()
            {
                IsSuccess = true,
                StatusCode = 204
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.GetByIdAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _postRepository.DeleteAsync(post));

            // Act
            var result = await _postService.DeletePostAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(204);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.DeletePostAsync(inexistentPostGuid);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePostAsync_WithNotPostCreator_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var antotherUser = A.Fake<User>();
            anotherProfile.User = antotherUser;

            var post = A.Fake<Post>();
            post.Profile = profile;

            BaseResponse response = new()
            {
                Message = "Only post creator can delete the post",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetByIdAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(anotherProfile));

            // Act
            var result = await _postService.DeletePostAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task LikePostAsync_ExistentAndPublicPost_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postLikes = A.Fake<ICollection<PostLike>>();
            post.Profile = profile;
            post.PostLikes = postLikes;

            BaseResponse response = new()
            {
                Message = "Post successfuly liked",
                IsSuccess = true,
                StatusCode = 200
            };

            PostLike like = new()
            {
                Post = post,
                Profile = profile
            };

            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.PostAlreadyLikedAsync(profile, post)).Returns(Task.FromResult(false));
            A.CallTo(() => _postRepository.AddPostLikeAsync(like)).Returns(Task.FromResult(like));

            // Act
            var result = await _postService.LikePostAsync(post.Id);

            // Assert 
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task LikePostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.LikePostAsync(inexistentPostGuid);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task LikePostAsync_AlreadyLikedPost_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postLikes = A.Fake<ICollection<PostLike>>();
            post.Profile = profile;
            post.PostLikes = postLikes;

            BaseResponse response = new()
            {
                Message = "You already liked this post.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.PostAlreadyLikedAsync(profile, post)).Returns(Task.FromResult(true));

            // Act
            var result = await _postService.LikePostAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UnlikePostAsync_ExistentAndValidPost_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postLike = A.Fake<PostLike>();
            var postLikes = A.Fake<ICollection<PostLike>>();
            post.Profile = profile;
            post.PostLikes = postLikes;

            BaseResponse response = new()
            {
                Message = "Post successfuly unliked",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.GetPostLikeAsync(profile, post)).Returns(Task.FromResult(postLike));
            A.CallTo(() => _postRepository.PostAlreadyLikedAsync(profile, post)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.RemovePostLikeAsync(postLike));

            // Act
            var result = await _postService.UnlikePostAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UnlikePostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UnlikePostAsync(inexistentPostGuid);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UnlikePostAsync_NotLikedPost_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postLikes = A.Fake<ICollection<PostLike>>();
            post.Profile = profile;
            post.PostLikes = postLikes;

            BaseResponse response = new()
            {
                Message = "You doens't like this post yet.",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.PostAlreadyLikedAsync(profile, post)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UnlikePostAsync(post.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        // ToDo => implement post comments tests
        [Fact]
        public async Task CommentPostAsync_ExistentAndValidPost_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postComments = A.Fake<ICollection<PostComment>>();
            post.Profile = profile;
            post.PostComments = postComments;

            BaseResponse response = new()
            {
                Message = "Comment successfuly added",
                IsSuccess = true,
                StatusCode = 201
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            PostComment comment = new()
            {
                Post = post,
                Comment = request.Comment
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.AddCommentPostAsync(comment)).Returns(Task.FromResult(comment));

            // Act
            var result = await _postService.CommentPostAsync(post.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CommentPostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            Guid inexistentPostGuid = Guid.NewGuid();

            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            CommentPostRequest request = new()
            {
                Comment = "Inexistente post"
            };

            A.CallTo(() => _postRepository.PostExistsAsync(inexistentPostGuid)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.CommentPostAsync(inexistentPostGuid, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateCommentAsync_ExistentAndValidPost_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postComments = A.Fake<ICollection<PostComment>>();
            post.Profile = profile;
            post.PostComments = postComments;

            BaseResponse response = new()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 201
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));

            // Act
            var result = await _postService.CommentPostAsync(post.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateCommentAsync_InexistentComment_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postComments = A.Fake<ICollection<PostComment>>();
            var comment = A.Fake<PostComment>();
            post.Profile = profile;
            post.PostComments = postComments;

            BaseResponse response = new()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 201
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            A.CallTo(() => _postRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UpdateCommentAsync(comment.Id, request);

            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteCommentAsync_ExistentAndValidComment_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var postComments = A.Fake<ICollection<PostComment>>();
            var comment = A.Fake<PostComment>();
            post.Profile = profile;
            post.PostComments = postComments;

            BaseResponse response = new()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 201
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));        
        }
    }
}