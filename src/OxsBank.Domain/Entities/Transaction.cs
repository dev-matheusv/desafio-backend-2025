namespace OxsBank.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public string Type { get; set; } = null!; // Saque, Depósito, Transferência
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
}