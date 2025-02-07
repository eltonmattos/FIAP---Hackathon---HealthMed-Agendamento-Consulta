using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    [CollectionDefinition("Test_MedicoDependent")]
    public class Test_Paciente_Login()
    {
        [Fact]
        public void EfetuaLoginComFalha()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            String? token = pacienteRepository.GetToken("ana.pereir@example.com", "Senha122@", false);
            Assert.Null(token);
        }

        [Fact]
        public void EfetuaLoginComSucesso()
        {
            PacienteRepository pacienteRepository = new(TestHelpers.GetConfiguration());
            String? token = pacienteRepository.GetToken("ana.pereira@example.com", "Senha123@", false);
            Assert.NotNull(token);
        }
    }

    [CollectionDefinition("Test_Paciente_LoginDependent")]
    public class Test_Paciente: IDisposable
    {
        [Fact]
        public void ListaMedicos()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            IEnumerable<object> medicos = medicoRepository.GetMedicos();
            Assert.IsAssignableFrom<IEnumerable<object>>(medicos);
        }

        [Fact]
        public void ListaMedicos_FiltroPorEspecialidade()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            IEnumerable<object> medicos = medicoRepository.GetMedicos("cardio");
            Assert.IsAssignableFrom<IEnumerable<object>>(medicos);
        }

        [Fact]
        public void AgendaConsultaComSucesso()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");
            Agendamento agendamento = new(new DateTime(2025, 02, 10, 09, 0, 0), new DateTime(2025, 02, 10, 09, 30, 0), idMedico, idPaciente);

            Guid idAgendamento = agendamentoRepository.Post(agendamento);
            Assert.True(idAgendamento != Guid.NewGuid());

            agendamentoRepository.Delete(idAgendamento);
        }

        [Fact]
        public void AgendaConsulta_SemHorarioDisponivel()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");
            Agendamento agendamento = new(new DateTime(2025, 02, 09, 10, 0, 0), new DateTime(2025, 02, 09, 10, 30, 0), idMedico, idPaciente);

            var expectedContent = "Horário não disponível";
            var exception = Assert.Throws<Exception>(() => agendamentoRepository.Post(agendamento));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void AgendaConsulta_ConsultaExistente()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440002");
            Guid idPaciente = new("660e8400-e29b-41d4-a716-446655440000");
            Agendamento agendamento = new(new DateTime(2025, 02, 10, 13, 00, 0), new DateTime(2025, 02, 10, 13, 30, 0), idMedico, idPaciente);

            var expectedContent = "Horário não disponível";
            var exception = Assert.Throws<Exception>(() => agendamentoRepository.Post(agendamento));
            Assert.Equal(expectedContent, exception.Message);
        }

        [Fact]
        public void CancelarAgendamento()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idAgendamento = new("74de47aa-8af9-4770-a2e7-072b019503ff");

            agendamentoRepository.AlterarStatusAgendamento(idAgendamento, (int)StatusAgendamento.RecusadoPeloMedico, "Motivo do Cancelamento");

            agendamentoRepository.ReativarAgendamento(idAgendamento);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
