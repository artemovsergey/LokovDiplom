using LokovApp.Models;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Получение списка всех пользователей
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserListItem>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Получение пользователя по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserData>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { success = false, message = "Пользователь не найден" });

        return Ok(user);
    }

    /// <summary>
    /// Создание нового пользователя (администратором)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserData>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Обновление данных пользователя
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserData>> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request
    )
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            if (user == null)
                return NotFound(new { success = false, message = "Пользователь не найден" });

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Блокировка/разблокировка пользователя
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleUserStatus(Guid id)
    {
        var result = await _userService.ToggleUserStatusAsync(id);
        if (!result)
            return NotFound(new { success = false, message = "Пользователь не найден" });

        return Ok(new { success = true, message = "Статус пользователя изменен" });
    }

    /// <summary>
    /// Удаление (деактивация) пользователя
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound(new { success = false, message = "Пользователь не найден" });

        return Ok(new { success = true, message = "Пользователь деактивирован" });
    }

    /// <summary>
    /// Сброс пароля пользователя (администратором)
    /// </summary>
    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult> ResetUserPassword(
        Guid id,
        [FromBody] ResetPasswordRequest request
    )
    {
        try
        {
            var result = await _userService.ResetUserPasswordAsync(id, request);
            if (!result)
                return NotFound(new { success = false, message = "Пользователь не найден" });

            return Ok(new { success = true, message = "Пароль пользователя сброшен" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Проверка существования пользователя
    /// </summary>
    [HttpGet("check-username")]
    public async Task<ActionResult> CheckUsername([FromQuery] string username)
    {
        var exists = await _userService.UserExistsAsync(username);
        return Ok(new { exists });
    }

    /// <summary>
    /// Проверка существования email
    /// </summary>
    [HttpGet("check-email")]
    public async Task<ActionResult> CheckEmail([FromQuery] string email)
    {
        var exists = await _userService.EmailExistsAsync(email);
        return Ok(new { exists });
    }
}
