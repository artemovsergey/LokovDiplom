using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LokovApp.Models;

public class ProjectDocument
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    public DocumentType Type { get; set; }

    [Required]
    [StringLength(100)]
    public string Number { get; set; } = string.Empty;

    [StringLength(500)]
    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User CreatedBy { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
}

public enum DocumentType
{
    CommercialOffer, // Коммерческое предложение
    Contract, // Договор
    AdditionalAgreement, // Дополнительное соглашение
    AcceptanceAct, // Акт приема-передачи
    KS2, // КС-2
    KS3, // КС-3
    Invoice, // Счет на оплату
    PhotoReport, // Фотоотчет
}
