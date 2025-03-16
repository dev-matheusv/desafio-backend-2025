namespace OxsBank.Infrastructure.Configurations;

public static class CnpjConfiguration
{
    public static string FormatCnpj(string cnpj)
    {
        // Remove todos os caracteres que não são números
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }
}