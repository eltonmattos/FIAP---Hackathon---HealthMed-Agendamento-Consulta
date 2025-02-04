using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class TestHelpers()
    {
        public static readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5292") };
        private const string _jsonMediaType = "application/json";
        //private const int _expectedMaxElapsedMilliseconds = 15000;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task AssertResponseWithContentAsync<T>(HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode,
            T expectedContent)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(_jsonMediaType, response.Content.Headers.ContentType?.MediaType);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedContent, await JsonSerializer.DeserializeAsync<T?>(
                await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions));
        }

        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile(Path.GetFullPath("..\\..\\..\\..\\HealthMed.API.AgendamentoConsulta\\appsettings.json"))
                .Build();
        }

        public static async Task<HttpResponseMessage> RequestToken(string uri)
        {

            var response = await TestHelpers._httpClient.PostAsync(uri, null);
            return response;
        }

        public static async Task<String?> GetToken(HttpResponseMessage response)
        {
            string message = await response.Content.ReadAsStringAsync();
            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            string? token = ob.First().Value.ToString();
            return token;
        }

        public static StringContent GetJsonStringContent<T>(T model)
            => new(JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);
    }
}
