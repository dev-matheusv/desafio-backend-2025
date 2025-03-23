using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OxsBank.Application.DTOs;
using OxsBank.Application.Interfaces;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class AccountService(OxsBankDbContext context, IReceitaWsService receitaWsService, ILogger<AccountService> logger) : IAccountService
{
    public async Task<AccountDto.Account> CreateAccountAsync(AccountDto.CreateAccount dto)
    {
        logger.LogInformation("Criando conta para o CNPJ: {Cnpj}", dto.Cnpj);
        
        if (context.Accounts.Any(a => a.Cnpj == dto.Cnpj))
        {
            logger.LogWarning("Tentativa de criação de conta com CNPJ já cadastrado: {Cnpj}", dto.Cnpj);
            throw new Exception("CNPJ já cadastrado");
        }

        var companyName = await receitaWsService.GetCompanyName(dto.Cnpj);
        if (string.IsNullOrEmpty(companyName))
        {
            logger.LogWarning("Tentativa de criação de conta com CNPJ inválido: {Cnpj}", dto.Cnpj);
            throw new Exception("CNPJ inválido ou não encontrado");
        }
        
        try
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = companyName,
                Cnpj = dto.Cnpj,
                AccountNumber =  Guid.NewGuid().ToString().Substring(0, 9), // Também fiz com Random somente para números 
                Agency = "0001",
                DocumentImage = $"uploads/{Guid.NewGuid()}.png"
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        
            logger.LogInformation("Conta criada com sucesso! ID: {AccountId}, CNPJ: {Cnpj}", account.Id, account.Cnpj);

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
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar conta com CNPJ: {Cnpj}", dto.Cnpj);
            throw new Exception("");
        }
    }

    public async Task<AccountDto.Account> GetAccountByIdAsync(Guid accountId)
    {
        logger.LogInformation("Consultando uma conta com o Id: {Id}", accountId);
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            logger.LogWarning("Tentativa de consultar uma conta com o Id: {Id}", accountId);
            throw new Exception("Conta não encontrada");
        }
        
        try
        {
            logger.LogInformation("Consulta concluída com sucesso! ID: {AccountId}, CNPJ: {Cnpj}", accountId, account.Cnpj);
        
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao consultar uma conta com o Id: {Id}", accountId);
            throw;
        }
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
        logger.LogInformation("Deletando uma conta com o Id: {Id}", accountId);
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            logger.LogWarning("Tentativa de deletar uma conta com o Id: {Id}", accountId);
            throw new Exception("Conta não encontrada.");
        }
    
        try
        {
            // Buscar transações associadas
            var transactions = await context.Transactions
                .Where(t => t.AccountId == accountId || t.DestinationAccountId == accountId)
                .ToListAsync();

            if (transactions.Any())
            {
                // Remover todas as transações associadas
                context.Transactions.RemoveRange(transactions);
                await context.SaveChangesAsync(); // Salva a remoção antes de excluir a conta
            }
            
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Conta deletada com sucesso! ID: {AccountId}, CNPJ: {Cnpj}", accountId, account.Cnpj);
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao deletar uma conta com o Id: {Id}", accountId);
            throw;
        }
    }
}