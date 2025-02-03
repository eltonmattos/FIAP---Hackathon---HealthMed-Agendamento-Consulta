using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Medico
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
        public async void CriaDisponibilidadeAgendamento()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }

        [Fact]
        public async void AlteraDisponibilidadeAgendamento()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginMedico/LoginMedico");
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }
    }
}
