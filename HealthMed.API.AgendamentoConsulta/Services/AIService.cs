using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace HealthMed.API.AgendamentoConsulta.Services
{
    public class Contents
    {
        [JsonProperty("parts")] // Mapeia a propriedade parts para "parts" no JSON
        public required Part[] Parts { get; set; }
    }

    public class Part
    {
        [JsonProperty("text")] // Mapeia a propriedade text para "text" no JSON
        public required string Text { get; set; }
        // Outros tipos de conteúdo podem ser adicionados aqui, como "image", "video", etc.
    }

    public class GeminiRequest
    {
        [JsonProperty("contents")] // Mapeia a propriedade contents para "contents" no JSON
        public Contents[] Contents { get; set; }

        public static string Serializar(GeminiRequest requisicao)
        {
            return JsonConvert.SerializeObject(requisicao, Formatting.Indented);
        }

        public static GeminiRequest Desserializar(string json)
        {
            return JsonConvert.DeserializeObject<GeminiRequest>(json);
        }
    }





    public class AIService
    {
        public required String apiKey { get; set; }
        public required String apiUrl { get; set; }
        [SetsRequiredMembers]
        public AIService(String api_key, String api_url)
        {
            this.apiKey = api_key;
            this.apiUrl = api_url;
        }
        public async Task<string> PromptApi(string prompt)
        {
            GeminiRequest requisicao = new GeminiRequest
            {
                Contents = new Contents[]
                {
                    new Contents
                    {
                        Parts = new Part[]
                        {
                            new Part { Text = prompt }
                        }
                    }
                }
            };

            // Serializar o objeto para JSON
            string json = GeminiRequest.Serializar(requisicao);
            System.Console.WriteLine(json);

            // Desserializar o JSON de volta para um objeto
            GeminiRequest requisicaoDesserializada = GeminiRequest.Desserializar(json);

            // Acessar os dados do objeto desserializado
            string pergunta = requisicaoDesserializada.Contents[0].Parts[0].Text;
            System.Console.WriteLine(pergunta);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}");
            var content = new StringContent(GeminiRequest.Serializar(requisicao));
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}