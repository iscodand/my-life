using MyLifeApp.Infrastructure.Identity.DTOs.Request;
using MyLifeApp.Infrastructure.Identity.DTOs.Response;
using MyLifeApp.Infrastructure.Identity.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyLifeApp.WebApi.Controllers
{
    [Route("api/v1/authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        public readonly IAuthenticationService _authenticationService;
        public readonly IProfileService _profileService;

        public AuthenticationController(IAuthenticationService userService, IProfileService profileService)
        {
            _authenticationService = userService;
            _profileService = profileService;
        }

        [HttpPost("register")]
        [ProducesResponseType(201, Type = typeof(RegisterUserResponse))]
        [ProducesResponseType(400, Type = typeof(RegisterUserResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest user)
        {
            if (ModelState.IsValid)
            {
                RegisterUserResponse response = await _authenticationService.RegisterAsync(user);

                if (response.IsSuccess)
                {
                    bool profile = await _profileService.CreateProfileAsync(response.Id);

                    if (profile)
                        return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(LoginUserResponse))]
        [ProducesResponseType(400, Type = typeof(LoginUserResponse))]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest user)
        {
            if (ModelState.IsValid)
            {
                LoginUserResponse response = await _authenticationService.LoginAsync(user);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [HttpPost("login/refresh")]
        [ProducesResponseType(200, Type = typeof(RefreshTokenResponse))]
        [ProducesResponseType(400, Type = typeof(RefreshTokenResponse))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (ModelState.IsValid)
            {
                RefreshTokenResponse response = await _authenticationService.RefreshTokenAsync(request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        // refactor => put this in ProfileController!
        [Authorize]
        [HttpPost("update-password")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _authenticationService.UpdatePasswordAsync(request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [HttpPost("forget-password")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _authenticationService.ForgetPasswordAsync(request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                BaseResponse response = await _authenticationService.ResetPasswordAsync(request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(500);
        }
    }
}
