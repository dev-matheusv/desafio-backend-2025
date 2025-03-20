using Microsoft.EntityFrameworkCore;
using OxsBank.Application.DTOs;
using OxsBank.Application.Interfaces;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class TransactionService(OxsBankDbContext context) : ITransactionService
{
    // Método de Saque
    public async Task<decimal> WithdrawAccountAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("O valor do saque deve ser maior que zero.");
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null) 
            throw new Exception("Conta não encontrada.");
        
        if (account.Balance < amount) 
            throw new Exception("Saldo insuficiente para efetuar o saque."); // verifica se o saldo é suficiente
        
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

        return account.Balance;
    }
    
    // Método de Depósito
    public async Task<decimal> DepositAccountAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
            throw new Exception("O valor do depósito deve ser maior que zero.");
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null) 
            throw new Exception("Conta não encontrada.");
        
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
        
        return account.Balance;
    }
    
    // Método de transferência
    public async Task<(decimal sourceBalance, decimal destinationBalance)> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            if (sourceAccountId == destinationAccountId)
                throw new Exception("Não é possível transferir para a mesma conta.");
                
            if (amount <= 0) 
                throw new Exception("O valor da transferência deve ser maior que zero.");
                
            var sourceAccount = await context.Accounts.FindAsync(sourceAccountId);
            var destinationAccount = await context.Accounts.FindAsync(destinationAccountId);

            switch (sourceAccount, destinationAccount)
            {
                case (null, null): throw new Exception("Ambas as contas não encontradas.");
                case (null, _): throw new Exception("Conta de origem não encontrada.");  
                case (_, null): throw new Exception("Conta de destino não encontrada.");
            }
                
            if (sourceAccount.Balance < amount) 
                throw new Exception("Saldo insuficiente para efetuar a transferência."); // Verifica se o saldo é suficiente
            try
            {
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

                return (sourceAccount.Balance, destinationAccount.Balance);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Ocorreu um erro ao concluir a transação, por favor tente novamente.");
            }
        }
    }
    
    // Método de Extrato
    public async Task<List<TransactionDto>> GetStatementAsync(Guid accountId)
    {
        var transactions = await context.Transactions
            .Include(t => t.Account)  // Carrega a conta associada à transação
            .Include(t => t.DestinationAccount)  // Carrega a conta de destino (se houver)
            .Where(t => t.AccountId == accountId || t.DestinationAccountId == accountId)
            .ToListAsync();

        // Mapeando as transações para o formato TransactionDto
        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.Type,
            AccountId = t.AccountId,
            DestinationAccountId = t.DestinationAccountId,
            Account = new AccountDto.Account
            {
                Id = t.AccountId,
                Name = t.Account.Name,
                Cnpj = t.Account.Cnpj,
                AccountNumber = t.Account.AccountNumber,
                Agency = t.Account.Agency,
                DocumentImage = t.Account.DocumentImage,
                Balance = t.Account.Balance
            }, // Mapeamento simplificado da conta
            DestinationAccount = t.DestinationAccountId != null ? new AccountDto.Account
            {
                Id = t.DestinationAccountId.Value,
                Name = t.DestinationAccount!.Name,
                Cnpj = t.DestinationAccount.Cnpj,
                AccountNumber = t.DestinationAccount!.AccountNumber,
                Agency = t.DestinationAccount!.Agency,
                DocumentImage = t.DestinationAccount!.DocumentImage,
                Balance = t.DestinationAccount!.Balance
            } : null,
        }).ToList();
    } 
}