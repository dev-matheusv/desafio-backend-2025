using System.ComponentModel.DataAnnotations;

namespace OxsBank.Application.Models;

public abstract class AccountModels
{
    public class CreateAccount
    {
        [Required]
        [MaxLength(14)]
        public string Cnpj { get; set; } = null!;

        [Required]
        public string DocumentImage { get; set; } = null!;
    }
    
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string Agency { get; set; } = null!;
        public string DocumentImage { get; set; } = null!;    
        
        public decimal Balance { get; set; } = 0;
    }
}