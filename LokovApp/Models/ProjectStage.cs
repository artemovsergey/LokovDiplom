using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class ProjectStage
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public int PlannedDays { get; set; }

    public int? ActualDays { get; set; }

    public StageStatus Status { get; set; } = StageStatus.Planned;

    public DateTime? CompletedAt { get; set; }

    [StringLength(1000)]
    public string? CompletionPhoto { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime? StartedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}

public enum StageStatus
{
    Planned, // Запланирован
    InProgress, // Выполняется
    Completed, // Завершен
    Delayed, // Задерживается
    Skipped, // Пропущен
}
