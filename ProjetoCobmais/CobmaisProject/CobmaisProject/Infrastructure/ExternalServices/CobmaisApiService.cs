using System.Text.Json;
using System.Text;

namespace CobmaisProject.Infrastructure.ExternalServices
{
    /// <summary>
    /// Serviço para interagir com a API externa Cobmais para atualizar informações de dívidas.
    /// </summary>
    public class CobmaisApiService
    {
        private readonly HttpClient _httpClient;

        public CobmaisApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> AtualizarDividaAsync(double valorOriginal, int diasAtraso, string tipoContrato)
        {

            var payload = new
            {
                TipoContrato = tipoContrato,
                Atraso = diasAtraso,
                Valor = valorOriginal
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://api.cobmais.com.br/testedev/calculo", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro na requisição. Status Code: {response.StatusCode}, Response: {errorContent}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Erro ao conectar com a API Cobmais: {ex.Message}";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erro inesperado: {ex.Message}";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
    }


        /// <summary>
        /// A classe ApiResponse reflete o formato de retorno da API externa e não representa
        /// um conceito de negócio ou domínio da aplicação. Por isso foi alocada dentro desta classe.
        /// </summary>
        public class ApiResponse
        {
            public string TipoContrato { get; set; }
            public int Atraso { get; set; }

            public double Valor { get; set; }
            public double ValorAtualizado { get; set; }
            public double DescontoMaximo { get; set; }
        }
    }
}
