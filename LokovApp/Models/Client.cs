using System.ComponentModel.DataAnnotations;

namespace LokovApp.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Patronymic { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ClientStatus Status { get; set; } = ClientStatus.Potential;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum ClientStatus
{
    Potential, // Потенциальный
    Active, // Активный
    Inactive, // Неактивный
    Completed, // Завершенный
}
