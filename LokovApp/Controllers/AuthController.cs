using LokovApp.Models;
using LokovApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

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

    /// <summary>
    /// Вход в систему
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = GetUserId();
        await _authService.LogoutAsync(userId);

        return Ok(new { success = true, message = "Выход выполнен успешно" });
    }

    /// <summary>
    /// Получение данных текущего пользователя
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserData>> GetCurrentUser()
    {
        var userId = GetUserId();
        var user = await _authService.GetCurrentUserAsync(userId);

        if (user == null)
            return NotFound(new { success = false, message = "Пользователь не найден" });

        return Ok(user);
    }

    /// <summary>
    /// Обновление профиля текущего пользователя
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<AuthResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request
    )
    {
        try
        {
            var userId = GetUserId();
            var user = await _authService.UpdateProfileAsync(userId, request);

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

    /// <summary>
    /// Смена пароля
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult<AuthResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request
    )
    {
        var userId = GetUserId();
        var response = await _authService.ChangePasswordAsync(userId, request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Проверка валидности токена
    /// </summary>
    [HttpGet("validate-token")]
    public ActionResult ValidateToken()
    {
        return Ok(new { success = true, message = "Токен действителен" });
    }

    private Guid GetUserId()
    {
        if (HttpContext.Items["UserId"] is Guid userId)
            return userId;

        throw new UnauthorizedAccessException("Пользователь не авторизован");
    }
}
