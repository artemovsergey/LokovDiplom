using System.Security.Cryptography;
using System.Text;
using LokovApp.Data;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services;

public interface IUserService
{
    Task<List<UserListItem>> GetAllUsersAsync();
    Task<UserData?> GetUserByIdAsync(Guid id);
    Task<UserData> CreateUserAsync(CreateUserRequest request);
    Task<UserData?> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> ToggleUserStatusAsync(Guid id);
    Task<bool> ResetUserPasswordAsync(Guid id, ResetPasswordRequest request);
    Task<bool> UserExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}

public class UserService : IUserService
{
    private readonly LokovAppContext _context;

    public UserService(LokovAppContext context)
    {
        _context = context;
    }

    public async Task<List<UserListItem>> GetAllUsersAsync()
    {
        return await _context
            .Users.Select(u => new UserListItem
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLogin = u.LastLogin,
                CreatedAt = u.CreatedAt,
                ProjectsCount = 0, // Можно добавить подсчет проектов
            })
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<UserData?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : MapToUserData(user);
    }

    public async Task<UserData> CreateUserAsync(CreateUserRequest request)
    {
        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Логин обязателен");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Пароль обязателен");

        if (request.Password.Length < 6)
            throw new ArgumentException("Пароль должен содержать минимум 6 символов");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Имя обязательно");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email обязателен");

        // Проверка уникальности
        if (await UserExistsAsync(request.Username))
            throw new InvalidOperationException("Пользователь с таким логином уже существует");

        if (await EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        // Определяем роль
        var role = Enum.TryParse<UserRole>(request.Role, true, out var parsedRole)
            ? parsedRole
            : UserRole.Manager;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = HashPassword(request.Password),
            FullName = request.FullName,
            Email = request.Email,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToUserData(user);
    }

    public async Task<UserData?> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        // Нельзя редактировать админа, если ты не админ
        // (эта проверка должна быть в контроллере)

        // Проверка уникальности email
        if (request.Email != user.Email && await EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        user.FullName = request.FullName;
        user.Email = request.Email;

        if (
            !string.IsNullOrWhiteSpace(request.Role)
            && Enum.TryParse<UserRole>(request.Role, true, out var role)
        )
        {
            user.Role = role;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await _context.SaveChangesAsync();
        return MapToUserData(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        // Физическое удаление вместо деактивации
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetUserPasswordAsync(Guid id, ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
            throw new ArgumentException("Пароли не совпадают");

        if (request.NewPassword.Length < 6)
            throw new ArgumentException("Пароль должен содержать минимум 6 символов");

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.PasswordHash = HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    #region Private Methods

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static UserData MapToUserData(User user)
    {
        return new UserData
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin,
            IsActive = user.IsActive,
        };
    }

    #endregion
}
