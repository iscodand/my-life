using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Interfaces.Services;

namespace MyLife.Api.Controllers
{
    [Route("api/v1/profile")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IProfileService _profileService;

        public ProfileController(IProfileRepository profileRepository, IProfileService profileService)
        {
            _profileRepository = profileRepository;
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
                GetFollowingsResponse response = await _profileRepository.GetProfileFollowings(profileUsername);

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
                GetFollowingsResponse response = await _profileRepository.GetProfileFollowers(profileUsername);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            return StatusCode(500);
        }

        [Authorize]
        [HttpPost("follow/{profileUsername}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> FollowProfile(string profileUsername)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _profileRepository.FollowProfile(profileUsername);

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
                BaseResponse response = await _profileRepository.UnfollowProfile(username);

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
