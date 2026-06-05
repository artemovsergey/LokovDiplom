using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class Project
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Number { get; set; } = string.Empty;

    public Guid ClientId { get; set; }

    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;

    [Required]
    public ProjectType Type { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Column(TypeName = "decimal(15,2)")]
    public decimal EstimatedCost { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal? ActualCost { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? PlannedEndDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.New;

    public Guid? BrigadeId { get; set; }

    [ForeignKey("BrigadeId")]
    public Brigade? Brigade { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public ICollection<ProjectStage> Stages { get; set; } = new List<ProjectStage>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<ProjectDocument> Documents { get; set; } = new List<ProjectDocument>();
    public ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
}

public enum ProjectType
{
    MajorRepair, // Капитальный ремонт
    PartialRepair, // Частичный ремонт
    RoofWorks, // Кровельные работы
    FacadeWorks, // Фасадные работы
    CombinedWorks, // Комплексные работы
}

public enum ProjectStatus
{
    New, // Новый
    Inspection, // Осмотр объекта
    Estimate, // Расчет сметы
    Approval, // Согласование
    Contract, // Договор подписан
    MaterialPurchase, // Закупка материалов
    InProgress, // В работе
    QualityControl, // Контроль качества
    Completed, // Завершен
    Warranty, // Гарантийное обслуживание
    Cancelled, // Отменен
}
