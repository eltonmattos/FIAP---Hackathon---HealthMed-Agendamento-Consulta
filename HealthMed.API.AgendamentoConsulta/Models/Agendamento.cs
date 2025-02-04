using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public enum StatusAgendamento
    {
        Solicitado = 0,
        Aprovado = 1,
        RecusadoPeloMedico = 2,
        CanceladoPeloPaciente = 3
    }
    public class Agendamento
    {
        protected Guid Id { get; set; }
        public required Guid IdPaciente { get; set; }
        public required Guid IdMedico { get; set; }
        public required DateTime DataInicio { get; set; }
        public required DateTime DataFim { get; set; }
        public required Int32 Status { get; set; }

        [SetsRequiredMembers]
        public Agendamento(DateTime DataInicio, DateTime DataFim, Guid IdMedico, Guid IdPaciente)
        {
            this.Id = Guid.NewGuid();
            this.IdPaciente = IdPaciente;
            this.IdMedico = IdMedico;
            this.DataInicio = DataInicio;
            this.DataFim = DataFim;
            this.Status = (Int32)StatusAgendamento.Solicitado;
        }
        public Agendamento() { }
    }
}
