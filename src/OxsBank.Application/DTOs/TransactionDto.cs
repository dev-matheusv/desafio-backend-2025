namespace OxsBank.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!; // "Deposit", "Withdraw", "Transfer"
    public Guid AccountId { get; set; }
    public Guid? DestinationAccountId { get; set; } // Para transferências
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public AccountDto.Account Account { get; set; } = null!;
    public AccountDto.Account? DestinationAccount { get; set; }
}