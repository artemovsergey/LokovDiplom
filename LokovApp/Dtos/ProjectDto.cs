using System.ComponentModel.DataAnnotations;

namespace LokovApp.DTOs;

public class CreateProjectDto
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal EstimatedCost { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public Guid? BrigadeId { get; set; }
}

public class UpdateProjectDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal EstimatedCost { get; set; }

    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public Guid? BrigadeId { get; set; }
}

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TypeDisplay { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Debt => EstimatedCost - PaidAmount;
    public int CompletionPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? BrigadeName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProjectFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public Guid? ClientId { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? StartTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
