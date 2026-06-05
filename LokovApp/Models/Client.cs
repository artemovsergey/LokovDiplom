using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class Client
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Patronymic { get; set; }

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(20)]
    public string? AdditionalPhone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public ClientSource Source { get; set; } = ClientSource.Other;

    public ClientStatus Status { get; set; } = ClientStatus.Potential;

    public ClientCategory Category { get; set; } = ClientCategory.Individual;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<ClientInteraction> Interactions { get; set; } =
        new List<ClientInteraction>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [NotMapped]
    public string FullName => $"{LastName} {FirstName} {Patronymic ?? ""}".Trim();
}

public enum ClientStatus
{
    Potential, // Потенциальный
    Active, // Активный
    Inactive, // Неактивный
    Regular, // Постоянный
    Archived, // Архивный
}

public enum ClientSource
{
    Recommendation, // Рекомендация
    Internet, // Интернет
    SocialMedia, // Социальные сети
    Advertisement, // Реклама
    DirectContact, // Прямое обращение
    Other, // Другое
}

public enum ClientCategory
{
    Individual, // Физическое лицо
    LegalEntity, // Юридическое лицо
    Entrepreneur, // ИП
}
