using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OxsBank.Application.Interfaces;

namespace OxsBank.Infrastructure.Services;

public class ReceitaWsService(HttpClient httpClient, ILogger<ReceitaWsService> logger) : IReceitaWsService
{
    private const string ReceitaWsUrl = "https://receitaws.com.br/v1/cnpj";  // URL da API ReceitaWS

    public async Task<string?> GetCompanyName(string cnpj)
    {
        try
        {
            var response = await httpClient.GetAsync($"{ReceitaWsUrl}/{cnpj}");

            // Se o código retornado for 429 é porque excedeu o limite de consultas por minuto
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                logger.LogWarning("Falha ao realizar a consulta pelo CNPJ, pois foi excedido o limite da API de 3 consultas por minuto.");
                throw new Exception("Não foi possível realizar a consulta pelo CNPJ, pois foi excedido o limite da API de 3 consultas por minuto. Aguarde 1 minuto e crie a conta novamente.");
            }

            var content = await response.Content.ReadAsStringAsync();

            // Parse do conteúdo JSON para pegar o nome da empresa (ajuste conforme a estrutura da resposta da API)
            var data = JsonConvert.DeserializeObject<dynamic>(content);
            return data?.nome;  // Ajuste com base na estrutura da resposta da API
        }
        catch (Exception)
        {
            // Em caso de erro, você pode logar ou lançar uma exceção
            return null;
        }
    }
}