using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Cache.Attributes;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Interfaces.Services;

namespace MyLife.Api.Controllers
{
    [Route("api/v1/posts")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICacheService _cacheService;

        public PostController(IPostService postService, ICacheService cacheService)
        {
            _postService = postService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [Cached(timeToLiveSeconds: 15)]
        public async Task<IActionResult> GetPublicPosts()
        {
            if (ModelState.IsValid)
            {
                GetAllPostsResponse response = await _postService.GetPublicPostsAsync();

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [HttpGet("{postId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [Cached(timeToLiveSeconds: 15)]
        public async Task<IActionResult> GetPost(int postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.GetPostByIdAsync(postId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [Cached(timeToLiveSeconds: 15)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.CreatePostAsync(request);

                if (response.IsSuccess)
                {
                    return StatusCode(201, response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPut("{postId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [Cached(timeToLiveSeconds: 15)]
        public async Task<IActionResult> UpdatePost(int postId, [FromBody] UpdatePostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.UpdatePostAsync(postId, request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpDelete("{postId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [Cached(timeToLiveSeconds: 15)]
        public async Task<IActionResult> DeletePost(int postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.DeletePostAsync(postId);

                if (response.IsSuccess)
                {
                    return NoContent();
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("{postId}/like")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> LikePost(int postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.LikePostAsync(postId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("{postId}/unlike")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.UnlikePostAsync(postId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("{postId}/comment")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> CommentPost(int postId, CommentPostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.CommentPostAsync(postId, request);

                if (response.IsSuccess)
                {
                    return StatusCode(201, response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPut("comment/{commentId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateComment(int commentId, CommentPostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.UpdateCommentAsync(commentId, request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpDelete("comment/{commentId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.DeleteCommentAsync(commentId);

                if (response.IsSuccess)
                {
                    return StatusCode(204, response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }
    }
}