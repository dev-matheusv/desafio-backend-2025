using Microsoft.EntityFrameworkCore;
using OxsBank.Application.Interfaces;
using OxsBank.Application.Models;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Configurations;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class AccountService(OxsBankDbContext context, IReceitaWsService receitaWsService) : IAccountService
{
    private readonly OxsBankDbContext _context = context;
    private readonly IReceitaWsService _receitaWsService = receitaWsService;

    public async Task<AccountModels.Account> CreateAccountAsync(AccountModels.CreateAccount model)
    {
        
        if (_context.Accounts.Any(a => a.Cnpj == model.Cnpj))
            throw new Exception("CNPJ já cadastrado");
        
        var companyName = await _receitaWsService.GetCompanyName(model.Cnpj);
        if (string.IsNullOrEmpty(companyName))
            throw new Exception("CNPJ inválido ou não encontrado");
        
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = companyName,
            Cnpj = model.Cnpj,
            AccountNumber = new Random().Next(100000000, 999999999).ToString(),
            Agency = "0001",
            DocumentImage = $"uploads/{Guid.NewGuid()}.png"
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountModels.Account
        {
            Id = account.Id,
            Name = account.Name,
            Cnpj = account.Cnpj,
            AccountNumber = account.AccountNumber,
            Agency = account.Agency,
            DocumentImage = account.DocumentImage
        };
    }

    public async Task<AccountModels.Account> GetAccountByIdAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) return null!;

        return new AccountModels.Account
        {
            Id = account.Id,
            Name = account.Name,
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
            Name = a.Name,
            Cnpj = a.Cnpj,
            AccountNumber = a.AccountNumber,
            Agency = a.Agency,
            DocumentImage = a.DocumentImage
        }).ToListAsync();
    }

    public async Task<bool> DeleteAccountAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) return false;
        
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
}