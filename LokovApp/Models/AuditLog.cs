using System.ComponentModel.DataAnnotations;

namespace LokovApp.Models;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    public Guid? EntityId { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(100)]
    public string? UserName { get; set; }

    [StringLength(4000)]
    public string? Details { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
