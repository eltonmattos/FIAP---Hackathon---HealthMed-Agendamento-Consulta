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
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HealthMed.API.AgendamentoConsulta.UnitTests
{
    public class RetornoDisponibilidadeMedico
    {
        public required string Id { get; set; }
        public required string Message { get; set; }

    }

    public class Test_Medico_Login
    {
        [Fact]
        public void EfetuaLoginComFalha()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            String? token = medicoRepository.GetToken("123456-SP", "Senha1223@", true);
            Assert.Null(token);
        }

        [Fact]
        public void EfetuaLoginComSucesso()
        {
            MedicoRepository medicoRepository = new(TestHelpers.GetConfiguration());
            String? token = medicoRepository.GetToken("123456-SP", "Senha123@", true);
            Assert.NotNull(token);
        }
    }
    [CollectionDefinition("Test_Medico_LoginDependent")]
    public class Test_Medico : IDisposable
    {
        [Fact]
        public void CriaDisponibilidadeAgendamento_DisponibilidadeCriadaComSucesso()
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades =
            [
                new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Saturday, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2025, 4, 1)),
                new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Saturday, new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0), new DateTime(2025, 4, 1)),
            ];

            IEnumerable<object>? retornoDisponibilidades = disponibilidadeMedicoRepository.Post(disponibilidades);

            Assert.IsAssignableFrom<IEnumerable<object>>(retornoDisponibilidades);

            foreach (object r in retornoDisponibilidades)
            {
                var property = r.GetType().GetProperty("id");
                if (property != null)
                {
                    object? guid = property.GetValue(r);
                    if (guid != null)
                    {
                        String? idRetorno = guid.ToString();
                        if (idRetorno != null)
                            disponibilidadeMedicoRepository.Delete(Guid.Parse(idRetorno));
                    }
                }
            }
        }

        [Fact]
        public void CriaDisponibilidadeAgendamento_SemDisponibilidadeHorario()
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades =
            [
                new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2025, 4, 1)),
            ];

            IEnumerable<object>? retornoDisponibilidades = disponibilidadeMedicoRepository.Post(disponibilidades);

            Assert.IsAssignableFrom<IEnumerable<object>>(retornoDisponibilidades);

            var expectedContent = "Horário de disponibilidade conflitante com outro horário já cadastrado.";

            foreach (object r in retornoDisponibilidades)
            {
                var property = r.GetType().GetProperty("message");
                if (property != null)
                {
                    object? guid = property.GetValue(r);
                    if (guid != null)
                    {
                        String? mensagemRetorno = guid.ToString();
                        if (mensagemRetorno != null)
                            Assert.Equal(expectedContent, mensagemRetorno);
                    }
                }
            }
        }


        [Fact]
        public void CriaDisponibilidadeAgendamento_IntervaloHorarioInvalido()
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades =
            [
                new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(12, 0, 0), new TimeSpan(11, 0, 0), new DateTime(2025, 4, 1)),
            ];

            IEnumerable<object>? retornoDisponibilidades = disponibilidadeMedicoRepository.Post(disponibilidades);

            Assert.IsAssignableFrom<IEnumerable<object>>(retornoDisponibilidades);

            var expectedContent = "Horário de Início não pode ser superior ao Horário de Fim.";

            foreach (object r in retornoDisponibilidades)
            {
                var property = r.GetType().GetProperty("message");
                if (property != null)
                {
                    object? guid = property.GetValue(r);
                    if (guid != null)
                    {
                        String? mensagemRetorno = guid.ToString();
                        if (mensagemRetorno != null)
                            Assert.Equal(expectedContent, mensagemRetorno);
                    }
                }
            }
        }

        [Fact]
        public void CriaDisponibilidadeAgendamento_DataValidadePassada()
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());

            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            List<DisponibilidadeMedico> disponibilidades =
            [
                new DisponibilidadeMedico(idMedico, (int)DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), new DateTime(2023, 4, 1)),
            ];

            IEnumerable<object>? retornoDisponibilidades = disponibilidadeMedicoRepository.Post(disponibilidades);

            Assert.IsAssignableFrom<IEnumerable<object>>(retornoDisponibilidades);

            var expectedContent = "Data de Validade não pode ser inferior a Data Atual.";

            foreach (object r in retornoDisponibilidades)
            {
                var property = r.GetType().GetProperty("message");
                if (property != null)
                {
                    object? guid = property.GetValue(r);
                    if (guid != null)
                    {
                        String? mensagemRetorno = guid.ToString();
                        if (mensagemRetorno != null)
                            Assert.Equal(expectedContent, mensagemRetorno);
                    }
                }
            }
        }

        [Fact]
        public void AlteraDisponibilidadeAgendamento_DisponibilidadeAlteradaComSucesso()
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(TestHelpers.GetConfiguration());

            Guid idDisponibilidadeMedico = new("665e0da5-b9e2-4bb3-8b53-2dcceca03a25");
            Guid idMedico = new("550e8400-e29b-41d4-a716-446655440000");
            DisponibilidadeMedico disponibilidadeMedico = new(idMedico,
                                                              (int)DayOfWeek.Saturday,
                                                              new TimeSpan(10, 0, 0),
                                                              new TimeSpan(12, 0, 0),
                                                              new DateTime(2025, 12, 31));

            disponibilidadeMedicoRepository.Put(idMedico.ToString(), idDisponibilidadeMedico.ToString(), disponibilidadeMedico);

            disponibilidadeMedico = new(idMedico,
                                                              (int)DayOfWeek.Friday,
                                                              new TimeSpan(13, 0, 0),
                                                              new TimeSpan(17, 0, 0),
                                                              new DateTime(2025, 12, 31));

            disponibilidadeMedicoRepository.Put(idMedico.ToString(), idDisponibilidadeMedico.ToString(), disponibilidadeMedico);
        }

        [Fact]
        public void AprovarAgendamento()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idAgendamento = new("35ddfab0-5496-46fd-95f3-43d651bb473b");

            agendamentoRepository.AlterarStatusAgendamento(idAgendamento, (int)StatusAgendamento.Aprovado);

            agendamentoRepository.ReativarAgendamento(idAgendamento);
        }

        [Fact]
        public void RecusarAgendamento()
        {
            AgendamentoRepository agendamentoRepository = new(TestHelpers.GetConfiguration());

            Guid idAgendamento = new("35ddfab0-5496-46fd-95f3-43d651bb473b");

            agendamentoRepository.AlterarStatusAgendamento(idAgendamento, (int)StatusAgendamento.RecusadoPeloMedico);

            agendamentoRepository.ReativarAgendamento(idAgendamento);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
