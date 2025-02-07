using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using HealthMed.API.AgendamentoConsulta.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class Test_Administrativo: IDisposable
    {
        [Fact]
        public void Paciente_POST_PacienteCadastradoComSucesso()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929046", "P@ssw0rd");

            Guid idPaciente = pacienteRepository.Post(paciente);

            Assert.True(idPaciente != Guid.NewGuid());

            if (paciente.Email!= null)
                pacienteRepository.Delete(email: paciente.Email);

        }

        [Fact]
        public void Paciente_POST_CadastrarPaciente_CPFJaExiste()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "52998224725", "P@ssw0rd");

            var exception = Assert.Throws<Exception>(() => pacienteRepository.Post(paciente));
            Assert.Equal("Paciente j� cadastrado", exception.Message);
        }

        [Fact]
        public void Paciente_POST_CadastrarPaciente_CPFInvalido()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929045", "P@ssw0rd");
            var exception = Assert.Throws<FormatException>(() => pacienteRepository.Post(paciente));
            Assert.Equal("CPF inv�lido", exception.Message);
        }


        [Fact]
        public void Paciente_POST_CadastrarPaciente_EmailJaExiste()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "ana.pereira@example.com", "73121929046", "P@ssw0rd");
            var exception = Assert.Throws<Exception>(() => pacienteRepository.Post(paciente));
            Assert.Equal("Paciente j� cadastrado", exception.Message);
        }

        [Fact]
        public void Paciente_POST_CadastrarPaciente_FormatoEmailInvalido()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331gmail.com", "73121929046", "P@ssw0rd");
            var exception = Assert.Throws<FormatException>(() => pacienteRepository.Post(paciente));
            Assert.Equal("Email inv�lido", exception.Message);
        }

        [Fact]
        public async void Paciente_POST_CadastrarPaciente_SenhaInvalida()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            Paciente paciente = new("Raphael Ribeiro", "raphaelribeiro331@gmail.com", "73121929046", "123");
            var exception = Assert.Throws<FormatException>(() => pacienteRepository.Post(paciente));

            var expectedContent = @"Comprimento m�nimo: A senha deve ser composta de:
                                            - Pelo menos 8 caracteres.
                                            - Pelo menos uma letra mai�scula.
                                            - Pelo menos uma letra min�scula.
                                            - Pelo menos um n�mero.";
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void Medico_POST_MedicoCadastradoComSucesso()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "35664039892", "012345-SP", "P@ssw0rd", 30, 100.00M);

            Guid idMedico = medicoRepository.Post(medico);
            Assert.True(idMedico != Guid.NewGuid());

            if (medico.Email != null)
                medicoRepository.Delete(email: medico.Email);
        }

        [Fact]
        public void Medico_POST_CadastrarMedico_CPFJaExiste()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "52998224725", "012345-SP", "P@ssw0rd", 30, 100.00M);
            var expectedContent = "M�dico j� cadastrado";
            var exception = Assert.Throws<Exception>(() => medicoRepository.Post(medico));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void Medico_POST_CadastrarMedico_CPFInvalido()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "35664039890", "012345-SP", "P@ssw0rd", 30, 100.00M);
            var expectedContent = "CPF inv�lido";
            var exception = Assert.Throws<FormatException>(() => medicoRepository.Post(medico));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void Medico_POST_CadastrarMedico_EmailJaExiste()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "joao.silva@example.com", "52998224725", "012345-SP", "P@ssw0rd", 30, 100.00M);
            var expectedContent = "M�dico j� cadastrado";
            var exception = Assert.Throws<Exception>(() => medicoRepository.Post(medico));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void Medico_POST_CadastrarMedico_SenhaInvalida()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "elton.mattos@outlook.com", "52998224725", "012345-SP", "1234", 30, 100.00M);
            var expectedContent = @"Comprimento m�nimo: A senha deve ser composta de:
                                            - Pelo menos 8 caracteres.
                                            - Pelo menos uma letra mai�scula.
                                            - Pelo menos uma letra min�scula.
                                            - Pelo menos um n�mero.";
            var exception = Assert.Throws<FormatException>(() => medicoRepository.Post(medico));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void Medico_POST_CadastrarMedico_FormatoEmailInvalido()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            Medico medico = new("Elton Mattos", "eltonmattosoutlook.com", "52998224725", "012345-SP", "P@ssw0rd", 30, 100.00M);
            var expectedContent = "Email inv�lido";
            var exception = Assert.Throws<FormatException>(() => medicoRepository.Post(medico));
            Assert.Equal(expectedContent, exception.Message);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}