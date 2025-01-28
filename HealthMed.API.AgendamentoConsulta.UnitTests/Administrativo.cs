using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public static class TestHelpers
    {
        private const string _jsonMediaType = "application/json";
        private const int _expectedMaxElapsedMilliseconds = 1000;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task AssertResponseWithContentAsync<T>(Stopwatch stopwatch,
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode,
            T expectedContent)
        {
            AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
            Assert.Equal(_jsonMediaType, response.Content.Headers.ContentType?.MediaType);
            Assert.Equal(expectedContent, await JsonSerializer.DeserializeAsync<T?>(
                await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions));
        }
        private static void AssertCommonResponseParts(Stopwatch stopwatch,
            HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(stopwatch.ElapsedMilliseconds < _expectedMaxElapsedMilliseconds);
        }
        public static StringContent GetJsonStringContent<T>(T model)
            => new(JsonSerializer.Serialize(model), Encoding.UTF8, _jsonMediaType);
    }

    public class Administrativo: IDisposable
    {

        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("") };

        [Fact]
        public void PacienteCadastrado()
        {

        }

        [Fact]
        public void CadastrarPaciente_CPFJaExiste()
        {

        }

        [Fact]
        public void CadastrarPaciente_CPFInvalido()
        {

        }


        [Fact]
        public void CadastrarPaciente_EmailJaExiste()
        {

        }

        [Fact]
        public void CadastrarPaciente_FormatoEmailInvalido()
        {

        }

        [Fact]
        public void CadastrarPaciente_SenhaInvalida()
        {

        }

        [Fact]
        public void MedicoCadastrado()
        {

        }

        [Fact]
        public void CadastrarMedico_CPFJaExiste()
        {

        }

        [Fact]
        public void CadastrarMedico_CPFInvalido()
        {

        }

        [Fact]
        public void CadastrarMedico_EmailJaExiste()
        {

        }

        [Fact]
        public void CadastrarMedico_SenhaInvalida()
        {

        }

        [Fact]
        public void MedicoCadastrado_FormatoEmailInvalido()
        {

        }

        [Fact]
        public void EmailAgendamentoEnviado()
        {

        }

        public void Dispose()
        {
            _httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}