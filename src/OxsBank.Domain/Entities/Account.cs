using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OxsBank.Domain.Entities;

public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(14)]
    public string Cnpj { get; set; } = null!;
    
    [Required]
    [MaxLength(10)]
    public string AccountNumber { get; set; } = null!;
    
    [Required]
    [MaxLength(5)]
    public string Agency { get; set; } = null!;
    public string DocumentImage { get; set; } = null!;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
}