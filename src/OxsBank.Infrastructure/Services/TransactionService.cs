using Microsoft.EntityFrameworkCore;
using OxsBank.Application.Interfaces;
using OxsBank.Application.Models;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class TransactionService(OxsBankDbContext context) : ITransactionService
{
    // Método de Saque
    public async Task<bool> WithdrawAccountAsync(Guid accountId, decimal amount)
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null) return false;
        
        if (account.Balance < amount) return false; // verifica se o saldo é suficiente
        
        account.Balance -= amount;
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Type = "Withdraw",
            AccountId = account.Id,
        };
        
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return true;
    }
    
    // Método de Depósito
    public async Task<bool> DepositAccountAsync(Guid accountId, decimal amount)
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null) return false;
        
        account.Balance += amount;
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Type = "Deposit",
            AccountId = account.Id,
        };
        
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        
        return true;
    }
    
    // Método de transferência
    public async Task<bool> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var sourceAccount = await context.Accounts.FindAsync(sourceAccountId);
                var destinationAccount = await context.Accounts.FindAsync(destinationAccountId);

                if (sourceAccount == null || destinationAccount == null) return false;
                if (sourceAccount.Balance < amount) return false; // Verifica se o saldo é suficiente

                sourceAccount.Balance -= amount;
                destinationAccount.Balance += amount;

                var transactionSource = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = "Transfer",
                    AccountId = sourceAccount.Id,
                    DestinationAccountId = destinationAccountId
                };

                var transactionDestination = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = "Transfer",
                    AccountId = destinationAccount.Id,
                    DestinationAccountId = sourceAccountId
                };

                context.Transactions.Add(transactionSource);
                context.Transactions.Add(transactionDestination);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
    
    // Método de Extrato
    public async Task<List<TransactionModels>> GetStatementAsync(Guid accountId)
    {
        var transactions = await context.Transactions
            .Where(t => t.AccountId == accountId || t.DestinationAccountId == accountId)
            .ToListAsync();

        // Mapeando as transações para o formato TransactionModels
        return transactions.Select(t => new TransactionModels
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.Type,
            AccountId = t.AccountId,
            DestinationAccountId = t.DestinationAccountId,
            Account = new AccountModels.Account { Id = t.AccountId }, // Mapeamento simplificado da conta
            DestinationAccount = t.DestinationAccountId != null ? new AccountModels.Account { Id = t.DestinationAccountId.Value } : null,
        }).ToList();
    } 
}