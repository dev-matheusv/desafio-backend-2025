using System.ComponentModel.DataAnnotations;

namespace OxsBank.Application.DTOs;

public abstract class AccountDto
{
    public class CreateAccount
    {
        [Required(ErrorMessage = "CNPJ inválido ou não encontrado.")]
        [MaxLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caractéres.")]
        public string Cnpj { get; set; } = null!;

        [Required(ErrorMessage = "O documento precisa ser informado no formato base64.")]
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