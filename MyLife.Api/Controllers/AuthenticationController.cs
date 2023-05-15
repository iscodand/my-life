using Identity.Infrastructure.Data.DTOs.Request;
using Identity.Infrastructure.Data.DTOs.Response;
using Identity.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/authentication")]
[ApiController]
public class AuthenticationController : Controller
{
    public readonly IUserRepository _userRepository;

    public AuthenticationController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    [ProducesResponseType(201, Type = typeof(BaseResponse))]
    [ProducesResponseType(400, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest user)
    {
        if (ModelState.IsValid)
        {
            BaseResponse response = await _userRepository.Register(user);

            if (response.IsSuccess)
            {
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