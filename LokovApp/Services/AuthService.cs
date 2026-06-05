using System.Security.Cryptography;
using System.Text;
using LokovApp.Data;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(AuthRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync(Guid userId);
    Task<AuthResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<UserData?> GetCurrentUserAsync(Guid userId);
    Task<UserData?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly LokovAppContext _context;

    public AuthService(LokovAppContext context)
    {
        _context = context;
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest request)
    {
        try
        {
            if (
                string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password)
            )
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Логин и пароль обязательны для заполнения",
                };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == request.Username.ToLower()
            );

            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "Неверный логин или пароль" };
            }

            if (!user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Учетная запись заблокирована. Обратитесь к администратору",
                };
            }

            // Проверка пароля
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Неверный логин или пароль" };
            }

            // Обновляем время последнего входа
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Генерируем простой токен для идентификации
            var token = GenerateSimpleToken(user.Id, request.RememberMe);

            return new AuthResponse
            {
                Success = true,
                Message = "Вход выполнен успешно",
                Token = token,
                User = MapToUserData(user),
            };
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = $"Ошибка при входе: {ex.Message}",
            };
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(request.Username))
                return new AuthResponse { Success = false, Message = "Логин обязателен" };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new AuthResponse { Success = false, Message = "Пароль обязателен" };

            if (request.Password != request.ConfirmPassword)
                return new AuthResponse { Success = false, Message = "Пароли не совпадают" };

            if (request.Password.Length < 6)
                return new AuthResponse
                {
                    Success = false,
                    Message = "Пароль должен содержать минимум 6 символов",
                };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new AuthResponse { Success = false, Message = "Имя обязательно" };

            if (string.IsNullOrWhiteSpace(request.Email))
                return new AuthResponse { Success = false, Message = "Email обязателен" };

            // Проверка уникальности
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == request.Username.ToLower()
            );

            if (existingUser != null)
                return new AuthResponse
                {
                    Success = false,
                    Message = "Пользователь с таким логином уже существует",
                };

            var existingEmail = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == request.Email.ToLower()
            );

            if (existingEmail != null)
                return new AuthResponse
                {
                    Success = false,
                    Message = "Пользователь с таким email уже существует",
                };

            // Создаем пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                Role = UserRole.Manager, // По умолчанию роль Manager
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Регистрация выполнена успешно",
                User = MapToUserData(user),
            };
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = $"Ошибка при регистрации: {ex.Message}",
            };
        }
    }

    public async Task LogoutAsync(Guid userId)
    {
        // В простой реализации без JWT просто логируем выход
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            // Можно добавить логирование выхода
            System.Diagnostics.Debug.WriteLine(
                $"User {user.Username} logged out at {DateTime.UtcNow}"
            );
        }
    }

    public async Task<AuthResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return new AuthResponse { Success = false, Message = "Текущий пароль обязателен" };

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return new AuthResponse { Success = false, Message = "Новый пароль обязателен" };

            if (request.NewPassword != request.ConfirmNewPassword)
                return new AuthResponse { Success = false, Message = "Новые пароли не совпадают" };

            if (request.NewPassword.Length < 6)
                return new AuthResponse
                {
                    Success = false,
                    Message = "Новый пароль должен содержать минимум 6 символов",
                };

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new AuthResponse { Success = false, Message = "Пользователь не найден" };

            // Проверяем текущий пароль
            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Неверный текущий пароль" };

            // Обновляем пароль
            user.PasswordHash = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "Пароль успешно изменен" };
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = $"Ошибка при смене пароля: {ex.Message}",
            };
        }
    }

    public async Task<UserData?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToUserData(user);
    }

    public async Task<UserData?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        // Проверка уникальности email
        if (request.Email != user.Email)
        {
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == request.Email.ToLower() && u.Id != userId
            );

            if (existingEmail != null)
                throw new InvalidOperationException("Пользователь с таким email уже существует");
        }

        user.FullName = request.FullName;
        user.Email = request.Email;

        await _context.SaveChangesAsync();
        return MapToUserData(user);
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == username.ToLower()
        );

        if (user == null || !user.IsActive)
            return false;

        return VerifyPassword(password, user.PasswordHash);
    }

    #region Private Methods

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }

    private static string GenerateSimpleToken(Guid userId, bool rememberMe)
    {
        // Простой токен: userId + timestamp + random
        var timestamp = DateTime.UtcNow.Ticks;
        var random = Guid.NewGuid().ToString("N")[..8];
        var tokenData = $"{userId}|{timestamp}|{random}|{rememberMe}";

        // Кодируем в Base64
        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(tokenBytes);
    }

    public static Guid? ExtractUserIdFromToken(string token)
    {
        try
        {
            var tokenBytes = Convert.FromBase64String(token);
            var tokenData = Encoding.UTF8.GetString(tokenBytes);
            var parts = tokenData.Split('|');

            if (parts.Length >= 1 && Guid.TryParse(parts[0], out var userId))
            {
                return userId;
            }
        }
        catch
        {
            // Игнорируем ошибки декодирования
        }

        return null;
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
