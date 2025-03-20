using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.Interfaces;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> WithdrawAccount(Guid accountId, [FromBody] decimal amount)
    {
        try
        {
            var balance = await transactionService.WithdrawAccountAsync(accountId, amount);
            return Ok(new { Message = $"Saque realizado com sucesso! O saldo atual é de: R${balance}" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> DepositAccount(Guid accountId, [FromBody] decimal amount)
    {
        try
        {
            var balance = await transactionService.DepositAccountAsync(accountId, amount);
            return Ok(new { Message = $"Depósito realizado com sucesso! O saldo atual é de: R${balance}" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{sourceAccountId}/transfer/{destinationAccountId}")]
    public async Task<IActionResult> TransferAccount(Guid sourceAccountId, Guid destinationAccountId,
        [FromBody] decimal amount)
    {
        try
        {
            var (sourceBalance, destinationBalance) = await transactionService.TransferAccountAsync(sourceAccountId, destinationAccountId, amount);
            return Ok(new { Message = $"Transferência realizado com sucesso! O saldo atual da conta de origem ({sourceAccountId}) é de: R${sourceBalance} " +
                                      $"e da conta destino ({destinationAccountId}) é de R${destinationBalance}" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> GetStatement(Guid accountId)
    {
        try
        {
            var transactions = await transactionService.GetStatementAsync(accountId);
            return Ok(transactions);
        }
        catch
        {
            return BadRequest("Ocorreu um erro ao carregar os extratos, por favor tente novamente.");
        }
    }
}