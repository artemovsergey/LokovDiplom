using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class ProjectExpense
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public ExpenseCategory Category { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;
}

public enum ExpenseCategory
{
    Materials, // Материалы
    Work, // Работы
    Transport, // Транспорт
    Overhead, // Накладные расходы
    Unexpected, // Непредвиденные
    Subcontractor, // Подрядчики
}
