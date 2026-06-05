namespace LokovApp.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project? Project { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public Guid? AssignedToId { get; set; }

    [ForeignKey("AssignedToId")]
    public User? AssignedTo { get; set; }

    public Guid? BrigadeId { get; set; }

    [ForeignKey("BrigadeId")]
    public Brigade? Brigade { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskStatus Status { get; set; } = TaskStatus.New;

    public DateTime? DueDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical,
}

public enum TaskStatus
{
    New,
    InProgress,
    OnReview,
    Completed,
    Cancelled,
}
