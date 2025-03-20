using OxsBank.Application.DTOs;

namespace OxsBank.Application.Interfaces;

public interface IAccountService
{
    Task<AccountDto.Account> CreateAccountAsync(AccountDto.CreateAccount dto);
    Task<AccountDto.Account> GetAccountByIdAsync(Guid id);
    Task<List<AccountDto.Account>>GetAllAccountsAsync();
    Task<bool> DeleteAccountAsync(Guid id);
}