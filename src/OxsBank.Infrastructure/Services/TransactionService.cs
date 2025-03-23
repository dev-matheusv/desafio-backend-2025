using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OxsBank.Application.DTOs;
using OxsBank.Application.Interfaces;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Services;

public class TransactionService(OxsBankDbContext context, ILogger<TransactionService> logger) : ITransactionService
{
    // Método de Saque
    public async Task<TransactionDto> WithdrawAccountAsync(Guid accountId, decimal amount)
    {
        logger.LogInformation("Efetuando saque no valor de R${amount}, na conta: {accountId}", amount, accountId);

        if (amount <= 0)
        {
            logger.LogWarning("Tentativa de efetuar saque de um valor menor que zero. valor: R${amount}, conta: {accountId}", amount, accountId);
            throw new Exception("O valor do saque deve ser maior que zero.");
        }
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            logger.LogWarning("Tentativa de efetuar saque em conta não encontrada. Conta: {accountId}", accountId);
            throw new Exception("Conta não encontrada.");
        }

        if (account.Balance < amount)
        {
            logger.LogWarning("Tentativa de efetuar saque para conta com valor insuficiente. Conta: {accountId}, valor do saque: R${amount}", accountId, amount);
            throw new Exception("Saldo insuficiente para efetuar o saque."); // verifica se o saldo é suficiente
        }

        try
        {
            account.Balance -= amount;
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = "Withdraw",
                AccountId = account.Id,
                CreatedAt = DateTime.UtcNow
            };
        
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            logger.LogInformation("Saque efetuado com sucesso! Conta: {accountId}, valor do saque: R${amount}", account.Id, amount);
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type,
                AccountId = transaction.AccountId,
                CreatedAt = transaction.CreatedAt,
                Account = new AccountDto.Account
                {
                    Id = account.Id,
                    Name = account.Name,
                    Cnpj = account.Cnpj,
                    AccountNumber = account.AccountNumber,
                    Agency = account.Agency,
                    Balance = account.Balance
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao efetuar saque para a conta: {accountId}, no valor de: R${amount}", accountId, amount);
            throw;
        }
    }
    
    // Método de Depósito
    public async Task<TransactionDto> DepositAccountAsync(Guid accountId, decimal amount)
    {
        logger.LogInformation("Efetuando depósito no valor de R${amount}, na conta: {accountId}", amount, accountId);
        
        if (amount <= 0)
        {
            logger.LogWarning("Tentativa de efetuar o depósito de um valor menor que zero. valor: R${amount}, conta: {accountId}", amount, accountId);
            throw new Exception("O valor do depósito deve ser maior que zero.");
        }
        
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            logger.LogWarning("Tentativa de efetuar um depósito em conta não encontrada. Conta: {accountId}", accountId);
            throw new Exception("Conta não encontrada.");
        }

        try
        {
            account.Balance += amount;
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = "Deposit",
                AccountId = account.Id,
                CreatedAt = DateTime.UtcNow
            };
        
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        
            logger.LogInformation("Depósito efetuado com sucesso! Conta: {accountId}, valor do saque: R${amount}", account.Id, amount);
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type,
                AccountId = transaction.AccountId,
                CreatedAt = transaction.CreatedAt,
                Account = new AccountDto.Account
                {
                    Id = account.Id,
                    Name = account.Name,
                    Cnpj = account.Cnpj,
                    AccountNumber = account.AccountNumber,
                    Agency = account.Agency,
                    Balance = account.Balance
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao efetuar um depósito para a conta: {accountId}, no valor de: R${amount}", accountId, amount);
            throw;
        }
    }
    
    // Método de transferência
    public async Task<TransactionDto> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId,
        decimal amount)
    {
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            logger.LogInformation("Efetuando transferência da conta: {sourceAccountId}, para a conta: {" +
                                  "destinationAccountId} no valor de R${amount}", sourceAccountId, destinationAccountId, amount);
            
            if (sourceAccountId == destinationAccountId)
            {
                logger.LogWarning("Tentativa de efetuar transferência para a mesma conta: {sourceAccountId}", sourceAccountId);
                throw new Exception("Não é possível transferir para a mesma conta.");
            }
                
            if (amount <= 0)
            {
                logger.LogWarning("Tentativa de efetuar transferência de um valor menor ou igual a zero. Valor: R${amount}", amount);
                throw new Exception("O valor da transferência deve ser maior que zero.");
            }
                
            var sourceAccount = await context.Accounts.FindAsync(sourceAccountId);
            var destinationAccount = await context.Accounts.FindAsync(destinationAccountId);

            switch (sourceAccount, destinationAccount)
            {
                case (null, null):
                {
                    logger.LogWarning("Tentativa de efetuar transferência, onde as contas: {sourceAccountId} e " +
                                      "{destinationAccountId} não foram encontradas.", sourceAccountId, destinationAccountId);
                    throw new Exception("Ambas as contas não encontradas.");
                }
                case (null, _):
                {
                    logger.LogWarning("Tentativa de efetuar transferência, onde a conta de origem: {sourceAccountId} não foi encontrada.", sourceAccountId);
                    throw new Exception("Conta de origem não encontrada.");
                }  
                case (_, null):
                {
                    logger.LogWarning("Tentativa de efetuar transferência, onde a conta de destino: {destinationAccountId} não foi encontrada.", destinationAccountId);
                    throw new Exception("Conta de destino não encontrada.");
                }
            }

            if (sourceAccount.Balance < amount)
            {
                logger.LogWarning("Tentativa de efetuar transferência com valor insuficiente. Conta: {sourceAccountId}, valor do saque: R${amount}", sourceAccountId, amount);
                throw new Exception("Saldo insuficiente para efetuar a transferência."); // Verifica se o saldo é suficiente
            }

            try
            {
                sourceAccount.Balance -= amount;
                destinationAccount.Balance += amount;

                var transactionSource = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = "Transfer",
                    CreatedAt = DateTime.UtcNow,
                    AccountId = sourceAccount.Id,
                    DestinationAccountId = destinationAccountId
                };

                var transactionDestination = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = "Transfer",
                    CreatedAt = DateTime.UtcNow,
                    AccountId = destinationAccount.Id,
                    DestinationAccountId = sourceAccountId
                };

                context.Transactions.Add(transactionSource);
                context.Transactions.Add(transactionDestination);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                logger.LogInformation("Transferência efetuada com sucesso! Conta de origem: {sourceAccountId}, conta de destino: " +
                                      "{destinationAccountId}, valor do saque: R${amount}", sourceAccountId, destinationAccountId, amount);
                
                return new TransactionDto
                {
                    Id = transactionSource.Id,
                    Amount = transactionSource.Amount,
                    Type = transactionSource.Type,
                    AccountId = transactionSource.AccountId,
                    Account = new AccountDto.Account
                    {
                        Id = sourceAccount.Id,
                        Name = sourceAccount.Name,
                        Cnpj = sourceAccount.Cnpj,
                        AccountNumber = sourceAccount.AccountNumber,
                        Agency = sourceAccount.Agency,
                        Balance = sourceAccount.Balance
                    },
                    DestinationAccountId = transactionDestination.Id,
                    DestinationAccount = new AccountDto.Account
                    {
                        Id = destinationAccount.Id,
                        Name = destinationAccount.Name,
                        Cnpj = destinationAccount.Cnpj,
                        AccountNumber = destinationAccount.AccountNumber,
                        Agency = destinationAccount.Agency,
                        Balance = destinationAccount.Balance
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Erro ao efetuar transferência da conta: {sourceAccountId}, para a conta: " +
                                    "{destinationAccountId}, no valor de: R${amount}", sourceAccountId, destinationAccountId, amount);
                
                throw new Exception("Ocorreu um erro ao concluir a transação, por favor tente novamente.");
            }
        }
    }
    
    // Método de Extrato
    public async Task<IEnumerable<TransactionDto>> GetStatementAsync(Guid accountId)
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