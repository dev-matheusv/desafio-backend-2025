using Microsoft.AspNetCore.Mvc;
using OxsBank.Application.Interfaces;
using OxsBank.Application.Models;
using OxsBank.Domain.Entities;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController(IAccountService accountService, OxsBankDbContext context, IReceitaWsService receitaWsService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly OxsBankDbContext _context = context;
    private readonly IReceitaWsService _receitaWsService = receitaWsService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountModels.CreateAccount model)
    {
        if (_context.Accounts.Any(a => a.Cnpj == model.Cnpj))
            return BadRequest("CNPJ já cadastrado");
        
        var companyName = await _receitaWsService.GetCompanyName(model.Cnpj);
        if (string.IsNullOrEmpty(companyName))
            return BadRequest("CNPJ inválido ou não encontrado");

        var newAccount = new Account
        {
            Name = companyName, // Obtido via ReceitaWS
            Cnpj = model.Cnpj,
            AccountNumber = new Random().Next(100000000, 999999999).ToString(),
            Agency = "0001",
            DocumentImage = model.DocumentImage
        };
        
        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Conta criada com sucesso!", AccountId = newAccount.Id });
        
        //var result = await _accountService.CreateAccountAsync(model);
        //return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var result = await _accountService.GetAccountByIdAsync(id);
        return true ? Ok(result) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var result = await _accountService.GetAllAccountsAsync();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var success = await _accountService.DeleteAccountAsync(id);
        return success ? NoContent() : NotFound();
    }
}