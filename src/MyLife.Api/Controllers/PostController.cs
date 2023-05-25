using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Interfaces.Services;

namespace MyLife.Api.Controllers
{
    [Route("api/v1/posts")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;

        public PostController(IPostRepository postRepository, IPostService postService)
        {
            _postRepository = postRepository;
            _postService = postService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAllPosts()
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postRepository.GetAllPosts();

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }

            return StatusCode(500);
        }

        [HttpGet("{postId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetPost(Guid postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postRepository.GetPostById(postId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postService.CreatePost(request);

                if (response.IsSuccess)
                {
                    return StatusCode(201, response);
                }

                return BadRequest(response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpPut("{postId}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Update(Guid postId, [FromBody] UpdatePostRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postRepository.UpdatePost(postId, request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpDelete("{postId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid postId)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _postRepository.DeletePost(postId);

                if (response.IsSuccess)
                {
                    return NoContent();
                }

                return NotFound();
            }

            return StatusCode(500);
        }
    }
}