using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using HealthMed.API.AgendamentoConsulta.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Administrativo: IDisposable
    {
        [Fact]
        public async void Paciente_POST_PacienteCadastradoComSucesso()
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

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email!= null)
                pacienteRepository.Delete(email: paciente.Email);

        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_CPFJaExiste()
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

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email != null)
                pacienteRepository.Delete(email: paciente.Email);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_CPFInvalido()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929045", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.OK; //Created (201)
            var sendContent = paciente;
            var expectedContent = "Paciente cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email != null)
                pacienteRepository.Delete(email: paciente.Email);
        }


        [Fact]
        public async void Paciente_POST_CadastrarPaciente_EmailJaExiste()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929045", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.OK; //Created (201)
            var sendContent = paciente;
            var expectedContent = "Paciente cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email != null)
                pacienteRepository.Delete(email: paciente.Email);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_FormatoEmailInvalido()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331gmail.com", "73121929045", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.OK; //Created (201)
            var sendContent = paciente;
            var expectedContent = "Paciente cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email != null)
                pacienteRepository.Delete(email: paciente.Email);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_SenhaInvalida()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929045", "123");
            var expectedStatusCode = System.Net.HttpStatusCode.OK; //Created (201)
            var sendContent = paciente;
            var expectedContent = "Paciente cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            if (paciente.Email != null)
                pacienteRepository.Delete(email: paciente.Email);
        }

        [Fact]
        public void Medico_POST_MedicoCadastradoComSucesso()
        {

        }

        [Fact]
        public void Medico_POST_CadastrarMedico_CPFJaExiste()
        {

        }

        [Fact]
        public void Medico_POST_CadastrarMedico_CPFInvalido()
        {

        }

        [Fact]
        public void Medico_POST_CadastrarMedico_EmailJaExiste()
        {

        }

        [Fact]
        public void Medico_POST_CadastrarMedico_SenhaInvalida()
        {

        }

        [Fact]
        public void Medico_POST_CadastrarMedico_FormatoEmailInvalido()
        {

        }

        public void Dispose()
        {
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}