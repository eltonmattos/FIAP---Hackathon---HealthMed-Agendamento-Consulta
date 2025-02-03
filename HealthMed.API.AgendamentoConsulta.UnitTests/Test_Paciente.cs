using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Paciente
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
        public async void ListaMedicos()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }

        [Fact]
        public async void AgendaConsultaComSucesso()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente"); ;
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }

        [Fact]
        public async void AgendaConsulta_SemHorarioDisponivel()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }

        [Fact]
        public async void AgendaConsulta_ConsultaExistente()
        {
            HttpResponseMessage response = await TestHelpers.RequestToken("joao.silva%40example.com", "Senha123%40", "api/Auth/LoginPaciente/LoginPaciente");
            String? token = await TestHelpers.GetToken(response);
            Assert.True(!String.IsNullOrEmpty(token));

            Assert.True(false);
        }
    }
}
