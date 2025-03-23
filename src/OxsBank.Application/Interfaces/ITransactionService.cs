using System.Data.SqlTypes;
using OxsBank.Application.DTOs;

namespace OxsBank.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> WithdrawAccountAsync(Guid accountId, decimal amount);
    Task<TransactionDto> DepositAccountAsync(Guid accountId, decimal amount);
    Task<TransactionDto> TransferAccountAsync(Guid sourceAccountId, Guid destinationAccountId, decimal amount);
    Task<IEnumerable<TransactionDto>> GetStatementAsync(Guid accountId);
}