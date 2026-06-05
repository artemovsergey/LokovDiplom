using System.ComponentModel.DataAnnotations;

namespace LokovApp.DTOs;

public class CreateClientDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Фамилия обязательна")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Patronymic { get; set; }

    [Required(ErrorMessage = "Телефон обязателен")]
    [Phone(ErrorMessage = "Неверный формат телефона")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(20)]
    public string? AdditionalPhone { get; set; }

    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [StringLength(200)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Адрес обязателен")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public string? Source { get; set; }
    public string? Category { get; set; }
}

public class UpdateClientDto
{
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

    [StringLength(20)]
    public string? AdditionalPhone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public string? Source { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
}

public class ClientResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AdditionalPhone { get; set; }
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ProjectsCount { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal Debt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ClientFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
