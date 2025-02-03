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
            var expectedStatusCode = System.Net.HttpStatusCode.OK; 
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
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "52998224725", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = paciente;
            var expectedContent = "Paciente já cadastrado";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_CPFInvalido()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929045", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = paciente;
            var expectedContent = "CPF inválido";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }


        [Fact]
        public async void Paciente_POST_CadastrarPaciente_EmailJaExiste()
        {
            Paciente paciente = new("Raphael Ribeiro", "ana.pereira@example.com", "73121929046", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = paciente;
            var expectedContent = "Paciente já cadastrado";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_FormatoEmailInvalido()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331gmail.com", "73121929046", "P@ssw0rd");
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = paciente;
            var expectedContent = "Email inválido";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_SenhaInvalida()
        {
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929046", "123");
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = paciente;
            var expectedContent = @"Comprimento mínimo: A senha deve ser composta de:
                                            - Pelo menos 8 caracteres.
                                            - Pelo menos uma letra maiúscula.
                                            - Pelo menos uma letra minúscula.
                                            - Pelo menos um número.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Paciente/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Medico_POST_MedicoCadastradoComSucesso()
        {
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "35664039892", "CRM/01234", "P@ssw0rd", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var sendContent = medico;
            var expectedContent = "Médico cadastrado com sucesso.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);

            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            if (medico.Email != null)
                medicoRepository.Delete(email: medico.Email);
        }

        [Fact]
        public async void Medico_POST_CadastrarMedico_CPFJaExiste()
        {
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "52998224725", "CRM/01234", "P@ssw0rd", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = medico;
            var expectedContent = "Médico já cadastrado";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Medico_POST_CadastrarMedico_CPFInvalido()
        {
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "35664039890", "CRM/01234", "P@ssw0rd", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = medico;
            var expectedContent = "CPF inválido";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Medico_POST_CadastrarMedico_EmailJaExiste()
        {
            Medico medico = new("Elton Mattos", "joao.silva@example.com", "52998224725", "CRM/01234", "P@ssw0rd", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = medico;
            var expectedContent = "Médico já cadastrado";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Medico_POST_CadastrarMedico_SenhaInvalida()
        {
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "52998224725", "CRM/01234", "1234", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = medico;
            var expectedContent = @"Comprimento mínimo: A senha deve ser composta de:
                                            - Pelo menos 8 caracteres.
                                            - Pelo menos uma letra maiúscula.
                                            - Pelo menos uma letra minúscula.
                                            - Pelo menos um número.";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        [Fact]
        public async void Medico_POST_CadastrarMedico_FormatoEmailInvalido()
        {
            Medico medico = new("Elton Mattos", "eltonmattosoutlook.com", "52998224725", "CRM/01234", "P@ssw0rd", 30);
            var expectedStatusCode = System.Net.HttpStatusCode.InternalServerError;
            var sendContent = medico;
            var expectedContent = "Email inválido";
            // Act.
            var response = await TestHelpers._httpClient.PostAsync("/api/Medico/", TestHelpers.GetJsonStringContent(sendContent));
            string message = await response.Content.ReadAsStringAsync();
            // Assert.
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.True(message.IndexOf(expectedContent) > 0);
        }

        public void Dispose()
        {
            TestHelpers._httpClient.DeleteAsync("/state").GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}