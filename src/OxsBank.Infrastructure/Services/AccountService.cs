using Microsoft.EntityFrameworkCore;
using OxsBank.Application.Interfaces;
using OxsBank.Application.Models;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class AccountService(OxsBankDbContext context) : IAccountService
{
    private readonly OxsBankDbContext _context = context;

    public async Task<AccountModels.Account> CreateAccountAsync(AccountModels.CreateAccount model)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Cnpj = model.Cnpj,
            DocumentImage = $"uploads/{Guid.NewGuid()}.png"
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountModels.Account
        {
            Id = account.Id,
            Name = "Empresa Teste",
            Cnpj = account.Cnpj,
            AccountNumber = account.AccountNumber,
            Agency = account.Agency,
            DocumentImage = account.DocumentImage
        };
    }

    public async Task<AccountModels.Account> GetAccountByIdAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return null!;

        return new AccountModels.Account
        {
            Id = account.Id,
            Name = "Empresa Teste",
            Cnpj = account.Cnpj,
            AccountNumber = account.AccountNumber,
            Agency = account.Agency,
            DocumentImage = account.DocumentImage
        };
    }

    public async Task<List<AccountModels.Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.Select(a => new AccountModels.Account
        {
            Id = a.Id,
            Name = "Empresa Teste",
            Cnpj = a.Cnpj,
            AccountNumber = a.AccountNumber,
            Agency = a.Agency,
            DocumentImage = a.DocumentImage
        }).ToListAsync();
    }

    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return false;
        
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
}