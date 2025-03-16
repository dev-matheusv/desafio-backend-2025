namespace OxsBank.Application.Interfaces;

public interface IReceitaWsService
{
    Task<string?> GetCompanyName(string cnpj);
}