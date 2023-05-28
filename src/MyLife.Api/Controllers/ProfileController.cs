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
                DetailProfileResponse response = await _profileService.GetAuthenticatedProfile();

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
                DetailProfileResponse response = await _profileService.GetProfileByUsername(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
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
                BaseResponse response = await _profileService.UpdateProfile(profileRequest);

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
                GetFollowingsResponse response = await _profileService.GetProfileFollowings(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
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
                GetFollowingsResponse response = await _profileService.GetProfileFollowers(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
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
                BaseResponse response = await _profileService.FollowProfile(username);

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
                BaseResponse response = await _profileService.UnfollowProfile(username);

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
