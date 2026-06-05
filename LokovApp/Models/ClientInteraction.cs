namespace LokovApp.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ClientInteraction
{
    [Key]
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;

    public InteractionType Type { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(1000)]
    public string? Result { get; set; }

    public DateTime InteractionDate { get; set; }

    public Guid CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}

public enum InteractionType
{
    Call, // Звонок
    Meeting, // Встреча
    Email, // Email
    Message, // Сообщение
    Comment, // Комментарий
}
