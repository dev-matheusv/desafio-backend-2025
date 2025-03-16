using OxsBank.Application.Models;

namespace OxsBank.Application.Interfaces;

public interface ITransactionService
{
    Task<bool> WithdrawAccountAsync(Guid accountId, decimal amount);
    Task<bool> DepositAccountAsync(Guid accountId, decimal amount);
    Task<bool> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount);
    Task<List<TransactionModels>> GetStatementAsync(Guid accountId);
}