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
                DetailProfileResponse response = await _profileRepository.GetProfileByUsername(profileUsername);

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
    }
}
