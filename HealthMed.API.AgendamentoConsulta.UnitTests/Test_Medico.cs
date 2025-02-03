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
    public class Test_Medico: IDisposable
    {
        [Fact]
        public async void EfetuaLoginComSucesso()
        {
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            // Act.
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
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
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha124%40", "api/Auth/LoginMedico/LoginMedico");
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_NaoAutorizado()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("joao.silva%40gmail.com", "Senha124%40", "api/Auth/LoginMedico/LoginMedico");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new DisponibilidadeMedico(idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(8, 0, 0),
                                                              new TimeSpan(12, 0, 0),
                                                              new DateTime(2025, 12, 31));
            Guid idDisponibilidade = disponibilidadeMedico.Id;
            var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
            var sendContent = disponibilidadeMedico;
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_DisponibilidadeCriadaComSucesso()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new DisponibilidadeMedico (idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(8, 0, 0),
                                                              new TimeSpan(12, 0, 0),
                                                              new DateTime(2025, 12, 31));
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidadeMedico;
            var expectedContent = "Período de Disponibilidade cadastrado com sucesso.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            ExpandoObject? ob = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(message);
            Guid idDisponibilidade = new Guid(ob.Last().Value.ToString());
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());
            disponibilidadeMedicoRepository.Delete(idDisponibilidade);
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_IntervaloHorarioInvalido()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new DisponibilidadeMedico(idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(12, 0, 0),
                                                              new TimeSpan(10, 0, 0),
                                                              new DateTime(2025, 12, 31));
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = disponibilidadeMedico;
            var expectedContent = "Horário de Início não pode ser superior ao Horário de Fim.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void CriaDisponibilidadeAgendamento_DataValidadePassada()
        {
            String? token = String.Empty;
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new DisponibilidadeMedico(idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(12, 0, 0),
                                                              new TimeSpan(10, 0, 0),
                                                              new DateTime(2024, 12, 31));
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = disponibilidadeMedico;
            var expectedContent = "Data de Validade não pode ser inferior a Data Atual.";
            // Act.
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PostAsync("/api/DisponibilidadeMedico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void AlteraDisponibilidadeAgendamento_DisponibilidadeAlteradaComSucesso()
        {
            String? token = String.Empty;
            Guid idDisponibilidadeMedico = new Guid("770e8400-e29b-41d4-a716-446655440009");
            HttpResponseMessage tokenResponse = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            token = await TestHelpers.GetToken(tokenResponse);

            Guid idMedico = new Guid("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new DisponibilidadeMedico(idMedico,
                                                              (int)DayOfWeek.Monday,
                                                              new TimeSpan(13, 0, 0),
                                                              new TimeSpan(16, 0, 0),
                                                              new DateTime(2025, 12, 31));
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = disponibilidadeMedico;
            var expectedContent = "Período de Disponibilidade cadastrado com sucesso.";
            // Act.
            
            TestHelpers._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await TestHelpers._httpClient.PutAsync($"/api/DisponibilidadeMedico/{idMedico}/{idDisponibilidadeMedico}", TestHelpers.GetJsonStringContent(sendContent));
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
