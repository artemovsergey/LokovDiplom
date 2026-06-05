using System.ComponentModel.DataAnnotations;

namespace LokovApp.Models;

public class Brigade
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ForemanName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    public int WorkersCount { get; set; } = 1;

    public BrigadeSpecialization Specialization { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

public enum BrigadeSpecialization
{
    Universal, // Универсальная
    Roofing, // Кровельная
    Facade, // Фасадная
    InteriorFinishing, // Внутренняя отделка
}
