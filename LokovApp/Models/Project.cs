using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int ClientId { get; set; }

    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;

    [Required]
    public ProjectType Type { get; set; }

    public decimal? Budget { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}

public enum ProjectType
{
    Roof, // Крыша
    Facade, // Фасад
    MajorRepair, // Капитальный ремонт
    Other, // Другое
}

public enum ProjectStatus
{
    Planned, // Запланирован
    InProgress, // В работе
    Completed, // Завершен
    Cancelled, // Отменен
}
