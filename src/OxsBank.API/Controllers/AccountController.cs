using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.DTOs;
using OxsBank.Application.Interfaces;
using OxsBank.Infrastructure.Configurations;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController(IAccountService accountService, ILogger<AccountController> logger) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccountAsync([FromBody] AccountDto.CreateAccount dto)
    {
        try
        {
            logger.LogInformation("Recebida requisição para criar conta. CNPJ: {Cnpj}", dto.Cnpj);
            
            dto.Cnpj = CnpjConfiguration.FormatCnpj(dto.Cnpj);
            var newAccount = await accountService.CreateAccountAsync(dto);
            return Ok(new { Message = "Conta criada com sucesso!", AccountId = newAccount.Id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar conta para CNPJ {Cnpj}", dto.Cnpj);
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        try
        {
            logger.LogInformation("Recebida requisição para consultar uma conta com o Id: {Id}", id);
            
            var result = await accountService.GetAccountByIdAsync(id);
            return Ok(new { Message = $"A consulta foi concluída com sucesso!", result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao consultar uma conta com o Id: {Id}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var result = await accountService.GetAllAccountsAsync();
        return Ok(new { Message = $"A consulta foi concluída com sucesso!", result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        try
        {
            logger.LogInformation("Recebida requisição para deletar uma conta com o Id: {Id}", id);
            
            await accountService.DeleteAccountAsync(id);
            return Ok(new { Message = "Conta deletada com sucesso!" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao deletar uma conta com o Id: {Id}", id);
            return BadRequest(ex.Message);
        }
    }
}