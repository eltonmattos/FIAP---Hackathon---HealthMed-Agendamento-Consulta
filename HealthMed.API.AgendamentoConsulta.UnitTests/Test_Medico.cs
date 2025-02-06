using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class RetornoDisponibilidadeMedico
    {
        public string id { get; set; }
        public string message { get; set; }

    }

    public class Test_Medico_Login
    {
        [Fact]
        public async void EfetuaLoginComSucesso()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);
            // Assert.
            Assert.Equal(expectedStatusCode, tokenResponse.StatusCode);
            Assert.True(!String.IsNullOrEmpty(token));
        }

        [Fact]
        public async void EfetuaLoginComFalha()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            // Act.
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha122%40");
            // Assert.
            Assert.Equal(expectedStatusCode, tokenResponse.StatusCode);
        }

    }
    [CollectionDefinition("Test_Medico_LoginDependent")]
    public class Test_Medico: IDisposable
    {
        Boolean emailNotify = false;  
        [Fact]
        public async void CriaDisponibilidadeAgendamento_NaoAutorizado()
        {
            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades = new List<DisponibilidadeMedico>();
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2025, 4, 1)));
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), new DateTime(2025, 4, 1)));

            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            var sendContent = disponibilidades;
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_DisponibilidadeCriadaComSucesso()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades = new List<DisponibilidadeMedico>();
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Saturday, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2025, 4, 1)));
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Saturday, new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), new DateTime(2025, 4, 1)));

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidades;
            var expectedContent = "Periodo de disponibilidade cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);

            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());
            List<RetornoDisponibilidadeMedico>? ob = System.Text.Json.JsonSerializer.Deserialize<List<RetornoDisponibilidadeMedico>>(message); ;
            foreach (var d in ob)
            {
                Assert.True(d.message == expectedContent);
                disponibilidadeMedicoRepository.Delete(new Guid(d.id));
            }
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_SemDisponibilidadeHorario()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades = new List<DisponibilidadeMedico>();
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2025, 4, 1)));
            
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidades;
            var expectedContent = "Horário de disponibilidade conflitante com outro horário já cadastrado.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);

            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());
            List<RetornoDisponibilidadeMedico>? ob = System.Text.Json.JsonSerializer.Deserialize<List<RetornoDisponibilidadeMedico>>(message);
            foreach (var d in ob)
            {
                Assert.True(d.message == expectedContent);
            }
        }


        [Fact]
        public async void CriaDisponibilidadeAgendamento_IntervaloHorarioInvalido()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades = new List<DisponibilidadeMedico>();
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(12, 0, 0), new TimeSpan(11, 0, 0), new DateTime(2025, 4, 1)));

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidades;
            var expectedContent = "Horário de Início não pode ser superior ao Horário de Fim.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);

            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());
            List<RetornoDisponibilidadeMedico>? ob = System.Text.Json.JsonSerializer.Deserialize<List<RetornoDisponibilidadeMedico>>(message); ;
            foreach (var d in ob)
            {
                Assert.True(d.message == expectedContent);
                disponibilidadeMedicoRepository.Delete(new Guid(d.id));
            }
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_DataValidadePassada()
        {
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades = new List<DisponibilidadeMedico>();
            disponibilidades.Add(new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2023, 4, 1)));

            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidades;
            var expectedContent = "Data de Validade não pode ser inferior a Data Atual.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);

            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());
            List<RetornoDisponibilidadeMedico>? ob = System.Text.Json.JsonSerializer.Deserialize<List<RetornoDisponibilidadeMedico>>(message); ;
            foreach (var d in ob)
            {
                Assert.True(d.message == expectedContent);
                disponibilidadeMedicoRepository.Delete(new Guid(d.id));
            }
        }

        [Fact]
        public async void AlteraDisponibilidadeAgendamento_DisponibilidadeAlteradaComSucesso()
        {
            Guid idDisponibilidadeMedico = new("770e8400-e29b-41d4-a716-446655440000");
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new(idMedico,
                                                              (int)DayOfWeek.Saturday,
                                                              new TimeSpan(10, 0, 0),
                                                              new TimeSpan(12, 0, 0),
                                                              new DateTime(2025, 12, 31)); var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidadeMedico;
            var expectedContent = "Período de Disponibilidade cadastrado com sucesso.";
            // Act.
            
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PutAsync($"/api/DisponibilidadeMedico/{idMedico}/{idDisponibilidadeMedico}", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            disponibilidadeMedico = new(idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(9, 0, 0),
                                                              new TimeSpan(12, 0, 0),
                                                              new DateTime(2025, 12, 31));
            new DisponibilidadeMedicoRepository(TestHelpers.GetConfiguration()).Put(idMedico.ToString(), idDisponibilidadeMedico.ToString(), disponibilidadeMedico);
        }

        [Fact]
        public async void AprovarAgendamento()
        {
            Guid idAgendamento = new("35ddfab0-5496-46fd-95f3-43d651bb473b");
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var expectedContent = "Agendamento aprovado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); 
            var response = await TestHelpers._httpClient.PutAsync($"/api/Agendamento/AprovarAgendamento/{idAgendamento}?notify={emailNotify}", null);
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            new AgendamentoRepository(TestHelpers.GetConfiguration()).ReativarAgendamento(idAgendamento);
        }

        [Fact]
        public async void RecusarAgendamento()
        {
            Guid idAgendamento = new("35ddfab0-5496-46fd-95f3-43d651bb473b");
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken(@$"Auth/LoginMedico/LoginMedico?crm=123456-SP&password=Senha123%40");
            String? token = await TestHelpers.GetToken(tokenResponse);
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var expectedContent = "Agendamento recusado.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PutAsync($"/api/Agendamento/RecusarAgendamento/{idAgendamento}?notify={emailNotify}", null);
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            new AgendamentoRepository(TestHelpers.GetConfiguration()).ReativarAgendamento(idAgendamento);
        }

        public void Dispose()
        {
            String? token = String.Empty;
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = null;
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}
