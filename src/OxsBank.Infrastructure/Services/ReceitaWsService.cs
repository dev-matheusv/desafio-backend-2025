using Newtonsoft.Json;
using OxsBank.Application.Interfaces;

namespace OxsBank.Infrastructure.Services;

public class ReceitaWsService(HttpClient httpClient) : IReceitaWsService
{
    private const string ReceitaWsUrl = "https://api.receitaws.com.br/v1/cnpj";  // URL da API ReceitaWS

    public async Task<string?> GetCompanyName(string cnpj)
    {
        try
        {
            var response = await httpClient.GetAsync($"{ReceitaWsUrl}/{cnpj}");

            if (!response.IsSuccessStatusCode)
            {
                // Caso a resposta não seja 200 OK, podemos lançar uma exceção ou retornar null
                return null;
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