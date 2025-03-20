using System.Data.SqlTypes;
using OxsBank.Application.DTOs;

namespace OxsBank.Application.Interfaces;

public interface ITransactionService
{
    Task<decimal> WithdrawAccountAsync(Guid accountId, decimal amount);
    Task<decimal> DepositAccountAsync(Guid accountId, decimal amount);
    Task<(decimal sourceBalance, decimal destinationBalance)> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount);
    Task<List<TransactionDto>> GetStatementAsync(Guid accountId);
}