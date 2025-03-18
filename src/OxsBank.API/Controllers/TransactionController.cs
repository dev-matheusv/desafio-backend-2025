using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.Interfaces;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost("{accountId}/withdraw")]
    public async Task<IActionResult> WithdrawAccount(Guid accountId, [FromBody] decimal amount)
    {
        if (amount <= 0) return BadRequest("O valor do saque deve ser maior que zero.");
        
        var success = await transactionService.WithdrawAccountAsync(accountId, amount);
        return success ? Ok(new { Message = "Saque realizado com sucesso!" }) : BadRequest("Saldo insuficiente ou conta não encontrada");
    }

    [HttpPost("{accountId}/deposit")]
    public async Task<IActionResult> DepositAccount(Guid accountId, [FromBody] decimal amount)
    {
        var success = await transactionService.DepositAccountAsync(accountId, amount);
        return success ? Ok(new { Message = "Depósito realizado com sucesso!" }) : BadRequest("Conta não encontrada");
    }

    [HttpPost("{sourceAccountId}/transfer/{destinationAccountId}")]
    public async Task<IActionResult> TransferAccount(Guid sourceAccountId, Guid destinationAccountId,
        [FromBody] decimal amount)
    {
        if (sourceAccountId == destinationAccountId) return BadRequest("Não é possível transferir para a mesma conta.");
        if (amount <= 0) return BadRequest("O valor da transferência deve ser maior que zero.");
        
        var success = await transactionService.TransferAccountAsync(sourceAccountId, destinationAccountId, amount);
        return success
            ? Ok(new { Message = "Transferência realizada com sucesso!" })
            : BadRequest("Transferência falhou. Verifique as contas e o saldo.");
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> GetStatement(Guid accountId)
    {
        var transactions = await transactionService.GetStatementAsync(accountId);
        return Ok(transactions);
    }
}