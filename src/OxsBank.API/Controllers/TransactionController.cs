using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.Interfaces;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger) : ControllerBase
{
    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> WithdrawAccount(Guid accountId, [FromBody] decimal amount)
    {
        try
        {
            logger.LogInformation("Recebida requisição para efetuar o saque da conta: {accountId} no valor de: R${amount}", accountId, amount);
            
            var transaction = await transactionService.WithdrawAccountAsync(accountId, amount);
            return Ok(new { Message = $"Saque no valor de R$:{amount} realizado com sucesso! O saldo atual é de: R${transaction.Account.Balance}" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao efetuar saque para a conta: {accountId}, no valor de: R${amount}", accountId, amount);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> DepositAccount(Guid accountId, [FromBody] decimal amount)
    {
        try
        {
            logger.LogInformation("Recebida requisição para efetuar o depósito para a conta: {accountId} no valor de: R${amount}", accountId, amount);
            
            var transaction = await transactionService.DepositAccountAsync(accountId, amount);
            return Ok(new { Message = $"Depósito no valor de R$:{amount} realizado com sucesso! O saldo atual é de: R${transaction.Account.Balance}" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao efetuar um depósito para a conta: {accountId}, no valor de: R${amount}", accountId, amount);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{sourceAccountId}/transfer/{destinationAccountId}")]
    public async Task<IActionResult> TransferAccount(Guid sourceAccountId, Guid destinationAccountId,
        [FromBody] decimal amount)
    {
        try
        {
            logger.LogInformation("Recebida requisição para efetuar a transferência da conta: {sourceAccountId}, para a conta: " +
                                  "{destinationAccountId}, no valor de: R${amount}", sourceAccountId, destinationAccountId, amount);
            
            var transactions = await transactionService.TransferAccountAsync(sourceAccountId, destinationAccountId, amount);
            return Ok(new { Message = $"Transferência realizada com sucesso! O saldo atual da conta de origem ({sourceAccountId}) é de: R${transactions.Account.Balance} " +
                                      $"e da conta destino ({destinationAccountId}) é de R${transactions.DestinationAccount!.Balance}", transactions });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao efetuar transferência da conta: {sourceAccountId}, para a conta: " +
                                "{destinationAccountId}, no valor de: R${amount}", sourceAccountId, destinationAccountId, amount);
            
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> GetStatement(Guid accountId)
    {
        try
        {
            logger.LogInformation("Recebida requisição para consultar o extrato da conta: {accountId}", accountId);
            var transactions = await transactionService.GetStatementAsync(accountId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao consultar o extrado da conta: {accountId}", accountId);
            return BadRequest("Ocorreu um erro ao carregar os extratos, por favor tente novamente.");
        }
    }
}