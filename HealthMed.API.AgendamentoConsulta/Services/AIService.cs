using Newtonsoft.Json;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Services
{
    public class AIService
    {
        private string api_key { get; set; }
        public AIService(String api_key)
        {
            this.api_key = api_key;
        }
        public async Task<string> PromptGeminiApi(string prompt)
        {
            // Substitua pela sua chave de API
            string apiKey = api_key;

            // URL da API do Gemini
            string apiUrl = "https://api.gemini.com/v1/completions";

            // Cria um objeto HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Adiciona o cabeçalho de autorização com a chave de API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Cria o corpo da solicitação com o prompt
                var requestBody = new
                {
                    model = "gemini-pro",
                    prompt = prompt,
                    max_tokens = 100, // Ajuste conforme necessário
                };

                // Converte o corpo da solicitação para JSON
                string json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Faz a solicitação POST para a API
                using (var response = await client.PostAsync(apiUrl, content))
                {
                    // Verifica se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        // Lê a resposta da API
                        string responseJson = await response.Content.ReadAsStringAsync();

                        // Deserializa a resposta JSON para um objeto
                        var responseData = JsonConvert.DeserializeObject<dynamic>(responseJson);

                        // Extrai o texto gerado da resposta
                        string generatedText = responseData.choices[0].text;

                        return generatedText;
                    }
                    else
                    {
                        // Lança uma exceção se a solicitação falhar
                        throw new Exception($"Erro ao chamar a API do Gemini: {response.StatusCode}");
                    }
                }
            }
        }
    }
}
