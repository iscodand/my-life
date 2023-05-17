using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Application.Interfaces;

namespace MyLife.Api.Controllers
{
    [Route("api/v1/profile")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(DetailProfileResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        public async Task<IActionResult> GetMyProfile()
        {
            if (ModelState.IsValid)
            {
                DetailProfileResponse response = await _profileRepository.GetAuthenticatedProfile();

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
                DetailProfileResponse response = await _profileRepository.GetProfile(profileUsername);

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
                BaseResponse response = await _profileRepository.UpdateProfile(profileRequest);
                
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
