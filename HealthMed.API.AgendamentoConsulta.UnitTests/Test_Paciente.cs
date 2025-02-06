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
    [CollectionDefinition("Test_MedicoDependent")]
    public class Test_Paciente_Login()
    {
        [Fact]
        public async void EfetuaLoginComFalha()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            // Act.
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha121%40");
            // Assert.
            Assert.Equal(expectedStatusCode, tokenResponse.StatusCode);
        }

        [Fact]
        public async void EfetuaLoginComSucesso()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);
            // Assert.
            Assert.Equal(expectedStatusCode, tokenResponse.StatusCode);
            Assert.True(!String.IsNullOrEmpty(token));
        }
    }

    [CollectionDefinition("Test_Paciente_LoginDependent")]
    public class Test_Paciente: IDisposable
    {
        Boolean emailNotify = false;

        [Fact]
        public async void ListaMedicos_NaoAutorizado()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha1223%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            // Act.
            var response = await TestHelpers._httpClient.GetAsync("/api/Medico/");
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void ListaMedicos()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.GetAsync("/api/Medico/");
            //string message = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void ListaMedicos_FiltroPorEspecialidade()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.GetAsync("/api/Medico/?especialidade=cardio");
            //string message = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void AgendaConsultaComSucesso_EmailDisparado()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 09, 0, 0), new DateTime(2025, 02, 10, 09, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = agendamento;
            var expectedContent = "Agendamento cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync($"/api/Agendamento?notify={emailNotify}", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);

            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            Guid idAgendamento = new(ob.Last().Value.ToString());
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());
            agendamentoRepository.Delete(idAgendamento);
        }

        [Fact]
        public async void CancelarAgendamento()
        {
            String? token = String.Empty;
            Guid idAgendamento = new("74de47aa-8af9-4770-a2e7-072b019503ff");
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            token = await TestHelpers.GetToken(tokenResponse);

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = "Motivo do Cancelamento";
            var expectedContent = "Agendamento cancelado.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PutAsync($"/api/Agendamento/CancelarAgendamento/{idAgendamento}?notify={emailNotify}", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            new AgendamentoRepository(TestHelpers.GetConfiguration()).ReativarAgendamento(idAgendamento);
        }

        [Fact]
        public async void AgendaConsulta_NaoAutorizado()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 10, 0, 0), new DateTime(2025, 02, 10, 10, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            var sendContent = agendamento;
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void AgendaConsulta_SemHorarioDisponivel()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 09, 10, 0, 0), new DateTime(2025, 02, 09, 10, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = agendamento;
            var expectedContent = "Horário não disponível";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void AgendaConsulta_ConsultaExistente()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"api/Auth/LoginPaciente/LoginPaciente?email=ana.pereira%40example.com&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");

            Agendamento agendamento = new(new DateTime(2025, 02, 10, 13, 00, 0), new DateTime(2025, 02, 10, 13, 30, 0), idMedico, idPaciente);

            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = agendamento;
            var expectedContent = "Horário não disponível";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/Agendamento/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }


        public void Dispose()
        {
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = null;
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}
