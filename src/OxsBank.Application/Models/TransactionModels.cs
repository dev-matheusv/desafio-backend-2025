namespace OxsBank.Application.Models;

public class TransactionModels
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!; // "Deposit", "Withdraw", "Transfer"
    public Guid AccountId { get; set; }
    public Guid? DestinationAccountId { get; set; } // Para transferências
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public AccountModels.Account Account { get; set; } = null!;
    public AccountModels.Account? DestinationAccount { get; set; }
}