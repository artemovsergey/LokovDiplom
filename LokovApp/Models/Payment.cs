namespace LokovApp.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;

    [Column(TypeName = "decimal(15,2)")]
    public decimal Amount { get; set; }

    public PaymentType Type { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;

    public DateTime PaymentDate { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User CreatedBy { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
}

public enum PaymentType
{
    Prepayment, // Предоплата
    Intermediate, // Промежуточный платеж
    FinalPayment, // Окончательный расчет
    AdditionalPayment, // Дополнительная оплата
}

public enum PaymentMethod
{
    Cash, // Наличные
    BankTransfer, // Банковский перевод
    Card, // Карта
}
