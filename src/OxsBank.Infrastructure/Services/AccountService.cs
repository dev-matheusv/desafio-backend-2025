using Microsoft.EntityFrameworkCore;
using OxsBank.Application.DTOs;
using OxsBank.Application.Interfaces;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class AccountService(OxsBankDbContext context, IReceitaWsService receitaWsService) : IAccountService
{
    public async Task<AccountDto.Account> CreateAccountAsync(AccountDto.CreateAccount dto)
    {
        
        if (context.Accounts.Any(a => a.Cnpj == dto.Cnpj))
            throw new Exception("CNPJ já cadastrado");
        
        var companyName = await receitaWsService.GetCompanyName(dto.Cnpj);
        if (string.IsNullOrEmpty(companyName))
            throw new Exception("CNPJ inválido ou não encontrado");
        
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = companyName,
            Cnpj = dto.Cnpj,
            AccountNumber = new Random().Next(100000000, 999999999).ToString(),
            Agency = "0001",
            DocumentImage = $"uploads/{Guid.NewGuid()}.png"
        };
        
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return new AccountDto.Account()
        {
            Id = account.Id,
            Name = account.Name,
            Cnpj = account.Cnpj,
            AccountNumber = account.AccountNumber,
            Agency = account.Agency,
            DocumentImage = account.DocumentImage
        };
    }

    public async Task<AccountDto.Account> GetAccountByIdAsync(Guid accountId)
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new Exception("Conta não encontrada");

        return new AccountDto.Account
        {
            Id = account.Id,
            Name = account.Name,
            Cnpj = account.Cnpj,
            AccountNumber = account.AccountNumber,
            Agency = account.Agency,
            DocumentImage = account.DocumentImage
        };
    }

    public async Task<List<AccountDto.Account>> GetAllAccountsAsync()
    {
        return await context.Accounts.Select(a => new AccountDto.Account
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
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new Exception("Conta não encontrada.");
        
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
        return true;
    }
}