using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Paciente: IDisposable
    {
        [Fact]
        public async void EfetuaLoginComSucesso()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            HttpResponseMessage response = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            String? token = await TestHelpers.GetToken(response);
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(!String.IsNullOrEmpty(token));
        }

        [Fact]
        public async void EfetuaLoginComFalha()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            // Act.
            HttpResponseMessage response = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha121%40", "api/Auth/LoginPaciente/LoginPaciente");
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void ListaMedicos_NaoAutorizado()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha121%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);
            
            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            // Act.
            var response = await TestHelpers._httpClient.GetAsync("/api/Medico/");
            string message = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void ListaMedicos()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.GetAsync("/api/Medico/");
            string message = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void AgendaConsultaComSucesso_EmailDisparado()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new Guid("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 10, 0, 0), new DateTime(2025, 02, 10, 10, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = agendamento;
            var expectedContent = "Agendamento cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            Guid idAgendamento = new Guid(ob.Last().Value.ToString());
            AgendamentoRepository agendamentoRepository = new AgendamentoRepository(TestHelpers.GetConfiguration());
            agendamentoRepository.Delete(idAgendamento);
        }


        [Fact]
        public async void AgendaConsulta_NaoAutorizado()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha124%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new Guid("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 10, 0, 0), new DateTime(2025, 02, 10, 10, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = agendamento;
            var expectedContent = "Agendamento cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void AgendaConsulta_SemHorarioDisponivel()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new Guid("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 09, 10, 0, 0), new DateTime(2025, 02, 09, 10, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = agendamento;
            var expectedContent = "Agendamento cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            Guid idAgendamento = new Guid(ob.Last().Value.ToString());
            AgendamentoRepository agendamentoRepository = new AgendamentoRepository(TestHelpers.GetConfiguration());
            agendamentoRepository.Delete(idAgendamento);
        }

        [Fact]
        public async void AgendaConsulta_ConsultaExistente()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("ana.pereira%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new Guid("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 13, 00, 0), new DateTime(2025, 02, 10, 13, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = agendamento;
            var expectedContent = "Agendamento cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            Guid idAgendamento = new Guid(ob.Last().Value.ToString());
            AgendamentoRepository agendamentoRepository = new AgendamentoRepository(TestHelpers.GetConfiguration());
            agendamentoRepository.Delete(idAgendamento);
        }


        public void Dispose()
        {
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = null;
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}
