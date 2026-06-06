using LokovApp.Models;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovCRM.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        // Без аутентификации просто возвращаем OK
        return Ok(new { success = true, message = "Выход выполнен успешно" });
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserData>> GetCurrentUser()
    {
        // Возвращаем дефолтного пользователя (админа)
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var user = await _authService.GetCurrentUserAsync(adminId);

        if (user == null)
            return NotFound(new { success = false, message = "Пользователь не найден" });

        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<AuthResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request
    )
    {
        try
        {
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user = await _authService.UpdateProfileAsync(adminId, request);

            if (user == null)
                return NotFound(
                    new AuthResponse { Success = false, Message = "Пользователь не найден" }
                );

            return Ok(
                new AuthResponse
                {
                    Success = true,
                    Message = "Профиль обновлен успешно",
                    User = user,
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new AuthResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<AuthResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request
    )
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var response = await _authService.ChangePasswordAsync(adminId, request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("validate-token")]
    public ActionResult ValidateToken()
    {
        return Ok(new { success = true, message = "Токен действителен" });
    }
}
