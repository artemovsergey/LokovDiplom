using System.ComponentModel.DataAnnotations;

namespace LokovApp.Dtos;

public class UploadPhotoDto
{
    [Required(ErrorMessage = "Файл обязателен")]
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Категория фото
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Привязка к этапу
    /// </summary>
    public Guid? StageId { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public int? SortOrder { get; set; }
}

public class UploadMultiplePhotosDto
{
    [Required(ErrorMessage = "Файлы обязательны")]
    public List<IFormFile> Files { get; set; } = new();

    public string? Category { get; set; }
    public string? Description { get; set; }
    public Guid? StageId { get; set; }
}

public class UpdatePhotoDto
{
    [StringLength(1000)]
    public string? Description { get; set; }

    public string? Category { get; set; }

    public int? SortOrder { get; set; }

    public Guid? StageId { get; set; }
}

public class PhotoResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectNumber { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Category { get; set; } = string.Empty;
    public string CategoryDisplay { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Description { get; set; }
    public Guid? StageId { get; set; }
    public string? StageName { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? TakenAt { get; set; }
}

public class PhotoFilterDto
{
    public string? Category { get; set; }
    public Guid? StageId { get; set; }
    public DateTime? UploadedFrom { get; set; }
    public DateTime? UploadedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
