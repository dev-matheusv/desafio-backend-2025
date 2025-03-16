using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OxsBank.Domain.Entities;

public class Transaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    public string Type { get; set; } = null!; // "Deposit", "Withdraw", "Transfer"
    
    [Required]
    public Guid AccountId { get; set; }
    
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
    
    public Guid? DestinationAccountId { get; set; } // Para transferências

    [ForeignKey("DestinationAccountId")]
    public Account? DestinationAccount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data de criação
}