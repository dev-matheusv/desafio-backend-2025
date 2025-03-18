using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.Interfaces;
using OxsBank.Application.Models;
using OxsBank.Infrastructure.Configurations;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccountAsync([FromBody] AccountModels.CreateAccount model)
    {
        try
        {
            model.Cnpj = CnpjConfiguration.FormatCnpj(model.Cnpj);
            var newAccount = await accountService.CreateAccountAsync(model);
            return Ok(new { Message = "Conta criada com sucesso!", AccountId = newAccount.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var result = await accountService.GetAccountByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var result = await accountService.GetAllAccountsAsync();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var success = await accountService.DeleteAccountAsync(id);
        return success ? NoContent() : NotFound();
    }
}