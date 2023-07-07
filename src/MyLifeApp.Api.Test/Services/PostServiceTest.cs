using Identity.Infrastructure.Models;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Services;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Api.Test
{
    public class PostServiceTest
    {
        private readonly IPostService _postService;
        private readonly IPostRepository _postRepository;
        private readonly IPostCommentRepository _postCommentRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticatedProfileService _authenticatedProfileService;

        public PostServiceTest()
        {
            _postRepository = A.Fake<IPostRepository>();
            _postCommentRepository = A.Fake<IPostCommentRepository>();
            _mapper = A.Fake<IMapper>();
            _authenticatedProfileService = A.Fake<IAuthenticatedProfileService>();
            _postService = new PostService(_postRepository, _postCommentRepository, _mapper, _authenticatedProfileService);
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
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();

            var post = A.Fake<Post>();
            var postLikes = A.Fake<ICollection<PostLike>>();
            var postComments = A.Fake<ICollection<PostComment>>();
            var postCommentsDTO = A.Fake<ICollection<GetPostCommentsDTO>>();

            profile.User = user;
            post.Profile = profile;
            post.PostLikes = postLikes;
            post.PostComments = postComments;

            var postDetail = A.Fake<DetailPostResponse>();
            var profileMapper = A.Fake<GetProfileResponse>();

            DetailPostResponse response = new()
            {
                Title = post.Title,
                Description = post.Description,
                Profile = profileMapper,
                Likes = post.PostLikes.Count,
                Comments = postCommentsDTO,
                Message = "Success",
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(true);
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _mapper.Map<DetailPostResponse>(post)).Returns(postDetail);
            A.CallTo(() => _mapper.Map<ICollection<GetPostCommentsDTO>>(postComments)).Returns(postCommentsDTO);
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
            DetailPostResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(false);

            // Act
            var result = await _postService.GetPostByIdAsync(101);

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

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UpdatePostAsync(101, request);

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
            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.DeletePostAsync(101);

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
            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.LikePostAsync(101);

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
            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UnlikePostAsync(101);

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
            var comment = A.Fake<PostComment>();
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

            A.CallTo(() => _postRepository.PostExistsAsync(post.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postRepository.GetPostDetailsAsync(post.Id)).Returns(Task.FromResult(post));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _mapper.Map<PostComment>(request)).Returns(comment);
            A.CallTo(() => _postCommentRepository.CreateAsync(comment)).Returns(Task.FromResult(comment));

            // Act
            var result = await _postService.CommentPostAsync(post.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(201);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CommentPostAsync_InexistentPost_ReturnsError()
        {
            // Arrange
            BaseResponse response = new()
            {
                Message = "Post not found",
                IsSuccess = false,
                StatusCode = 404
            };

            CommentPostRequest request = new()
            {
                Comment = "Inexistent post"
            };

            A.CallTo(() => _postRepository.PostExistsAsync(A<int>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.CommentPostAsync(101, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateCommentAsync_ExistentAndValidComment_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var comment = A.Fake<PostComment>();
            post.Profile = profile;
            comment.Profile = profile;

            BaseResponse response = new()
            {
                Message = "Comment successfuly updated",
                IsSuccess = true,
                StatusCode = 200
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postCommentRepository.GetByIdAsync(comment.Id)).Returns(Task.FromResult(comment));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _mapper.Map<PostComment>(request)).Returns(comment);

            // Act
            var result = await _postService.UpdateCommentAsync(comment.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(200);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateCommentAsync_UpdateAnotherUserComment_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var anotherUser = A.Fake<User>();
            anotherProfile.User = anotherUser;

            var post = A.Fake<Post>();
            var comment = A.Fake<PostComment>();
            comment.Profile = profile;
            post.Profile = profile;

            BaseResponse response = new()
            {
                Message = "Only comment author can update the comment",
                IsSuccess = false,
                StatusCode = 400
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postCommentRepository.GetByIdAsync(comment.Id)).Returns(Task.FromResult(comment));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(anotherProfile));

            // Act
            var result = await _postService.UpdateCommentAsync(comment.Id, request);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
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
                Message = "Comment not found",
                IsSuccess = false,
                StatusCode = 404
            };

            CommentPostRequest request = new()
            {
                Comment = "Nice post!"
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.UpdateCommentAsync(comment.Id, request);

            // Assert
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
            post.Profile = profile;

            var comment = A.Fake<PostComment>();
            comment.Post = post;
            comment.Profile = profile;

            BaseResponse response = new()
            {
                IsSuccess = true,
                StatusCode = 204
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postCommentRepository.GetByIdAsync(comment.Id)).Returns(Task.FromResult(comment));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postCommentRepository.DeleteAsync(comment));

            // Act
            var result = await _postService.DeleteCommentAsync(comment.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(204);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCommentAsync_PostAuthorCanDeleteComment_ReturnsSuccess()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var anotherUser = A.Fake<User>();
            anotherProfile.User = anotherUser;

            // Profile is post author
            var post = A.Fake<Post>();
            post.Profile = profile;

            // Another Profile is comment author
            var comment = A.Fake<PostComment>();
            comment.Post = post;
            comment.Profile = anotherProfile;

            BaseResponse response = new()
            {
                IsSuccess = true,
                StatusCode = 204
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(profile));
            A.CallTo(() => _postCommentRepository.GetByIdAsync(comment.Id)).Returns(Task.FromResult(comment));

            // Act
            var result = await _postService.DeleteCommentAsync(comment.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(204);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCommentAsync_DeleteAnotherUserComment_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var anotherProfile = A.Fake<Profile>();
            var anotherUser = A.Fake<User>();
            anotherProfile.User = anotherUser;

            var post = A.Fake<Post>();
            post.Profile = profile;

            var comment = A.Fake<PostComment>();
            comment.Profile = profile;
            comment.Post = post;

            BaseResponse response = new()
            {
                Message = "Only Comment Author or Post Author can delete the comment",
                IsSuccess = false,
                StatusCode = 400
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(true));
            A.CallTo(() => _postCommentRepository.GetByIdAsync(comment.Id)).Returns(Task.FromResult(comment));
            A.CallTo(() => _authenticatedProfileService.GetAuthenticatedProfile()).Returns(Task.FromResult(anotherProfile));

            // Act
            var result = await _postService.DeleteCommentAsync(comment.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(400);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteCommentAsync_InexistentComment_ReturnsError()
        {
            // Arrange
            var profile = A.Fake<Profile>();
            var user = A.Fake<User>();
            profile.User = user;

            var post = A.Fake<Post>();
            var comment = A.Fake<PostComment>();
            post.Profile = profile;
            comment.Profile = profile;

            BaseResponse response = new()
            {
                Message = "Comment not found",
                IsSuccess = false,
                StatusCode = 404
            };

            A.CallTo(() => _postCommentRepository.PostCommentExistsAsync(comment.Id)).Returns(Task.FromResult(false));

            // Act
            var result = await _postService.DeleteCommentAsync(comment.Id);

            // Assert
            result.Should().BeEquivalentTo(response);
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }
    }
}