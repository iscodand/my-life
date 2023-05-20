using Identity.Infrastructure.DTOs.Request;
using Identity.Infrastructure.DTOs.Response;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MyLifeApp.Application.Interfaces;

[Route("api/v1/authentication")]
[ApiController]
public class AuthenticationController : Controller
{
    public readonly IUserRepository _userRepository;
    public readonly IProfileRepository _profileRepository;

    public AuthenticationController(IUserRepository userRepository, IProfileRepository profileRepository)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    [HttpPost("register")]
    [ProducesResponseType(201, Type = typeof(RegisterUserResponse))]
    [ProducesResponseType(400, Type = typeof(RegisterUserResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest user)
    {
        if (ModelState.IsValid)
        {
            RegisterUserResponse response = await _userRepository.Register(user);
            bool profile = _profileRepository.RegisterProfile(response.Id);

            if (response.IsSuccess)
            {
                if (profile)
                    return Ok(response);
            }

            return BadRequest(response);
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
            LoginUserResponse response = await _userRepository.Login(user);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        return StatusCode(500);
    }

    [HttpPost("login/refresh")]
    [ProducesResponseType(200, Type = typeof(LoginUserResponse))]
    [ProducesResponseType(400, Type = typeof(LoginUserResponse))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (ModelState.IsValid)
        {
            RefreshTokenResponse response = await _userRepository.RefreshToken(request);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        return StatusCode(500);
    }
}