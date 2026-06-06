using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class ProjectPhoto
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public PhotoCategory Category { get; set; } = PhotoCategory.WorkProgress;

    public int SortOrder { get; set; } = 0;

    [StringLength(1000)]
    public string? Description { get; set; }

    public Guid? StageId { get; set; }

    [ForeignKey("StageId")]
    public ProjectStage? Stage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Делаем nullable
    public Guid? UploadedById { get; set; }

    [ForeignKey("UploadedById")]
    public User? UploadedBy { get; set; }

    [StringLength(500)]
    public string? ThumbnailFileName { get; set; }

    [StringLength(1000)]
    public string? ThumbnailPath { get; set; }

    public bool IsDeleted { get; set; } = false;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? TakenAt { get; set; }
}

public enum PhotoCategory
{
    Before, // До начала работ
    WorkProgress, // Процесс работы
    After, // После завершения
    Defect, // Дефекты
    Materials, // Материалы
    Documentation, // Документация
    Other, // Другое
}
