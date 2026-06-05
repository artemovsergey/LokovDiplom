namespace LokovApp.Models;

using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Manager;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool TwoFactorEnabled { get; set; } = false;

    // Navigation properties
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<ClientInteraction> Interactions { get; set; } =
        new List<ClientInteraction>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}

public enum UserRole
{
    Admin, // Администратор
    Manager, // Менеджер
    Foreman, // Прораб
    Accountant, // Бухгалтер
    Brigadier, // Бригадир
}
