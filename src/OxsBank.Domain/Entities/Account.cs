namespace OxsBank.Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string Agency { get; set; } = null!;
    public string DocumentImage { get; set; } = null!;
}