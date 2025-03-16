using OxsBank.Application.Models;

namespace OxsBank.Application.Interfaces;

public interface IAccountService
{
    Task<AccountModels.Account> CreateAccountAsync(AccountModels.CreateAccount model);
    Task<AccountModels.Account> GetAccountByIdAsync(Guid id);
    Task<List<AccountModels.Account>>GetAllAccountsAsync();
    Task<bool> DeleteAccountAsync(Guid id);
}