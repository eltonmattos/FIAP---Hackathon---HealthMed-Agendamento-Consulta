using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Administrativo: IDisposable
    {
        

        [Fact]
        public async void POSTPaciente_PacienteCadastradoComSucesso()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929046", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.OK; //Created (201)
            var sendContent = paciente;
            var expectedContent = "Paciente cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

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
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}