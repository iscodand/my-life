using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces.Services;

namespace MyLife.Api.Controllers
{
    [Route("api/v1/profile")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(DetailProfileResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetAuthenticatedProfile()
        {
            if (ModelState.IsValid)
            {
                DetailProfileResponse response = await _profileService.GetAuthenticatedProfileAsync();

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }

            return StatusCode(500);
        }

        [HttpGet("{profileUsername}")]
        [ProducesResponseType(200, Type = typeof(DetailProfileResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetProfileByUsername(string profileUsername)
        {
            if (ModelState.IsValid)
            {
                DetailProfileResponse response = await _profileService.GetProfileByUsernameAsync(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return NotFound();
            }
            return StatusCode(500);
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest profileRequest)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _profileService.UpdateProfileAsync(profileRequest);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            return StatusCode(500);
        }

        [HttpGet("{profileUsername}/following")]
        [ProducesResponseType(200, Type = typeof(DetailProfileResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetProfileFollowings(string profileUsername)
        {
            if (ModelState.IsValid)
            {
                GetFollowingsResponse response = await _profileService.GetProfileFollowingsAsync(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return NotFound();
            }
            return StatusCode(500);
        }

        [HttpGet("{profileUsername}/followers")]
        [ProducesResponseType(200, Type = typeof(DetailProfileResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetProfileFollowers(string profileUsername)
        {
            if (ModelState.IsValid)
            {
                GetFollowingsResponse response = await _profileService.GetProfileFollowersAsync(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return NotFound();
            }
            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("follow/{username}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> FollowProfile(string username)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _profileService.FollowProfileAsync(username);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("unfollow/{username}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UnfollowProfile(string username)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _profileService.UnfollowProfileAsync(username);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            return StatusCode(500);
        }
    }
}
