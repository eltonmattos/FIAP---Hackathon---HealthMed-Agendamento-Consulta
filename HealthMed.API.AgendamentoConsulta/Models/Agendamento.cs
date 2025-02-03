using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Agendamento
    {
        public Guid Id { get; set; }
        public required Guid IdPaciente { get; set; }
        public required Guid IdMedico { get; set; }
        public required DateTime DataInicio { get; set; }
        public required DateTime DataFim { get; set; }

        [SetsRequiredMembers]
        public Agendamento(DateTime DataInicio, DateTime DataFim, Guid IdMedico, Guid IdPaciente)
        {
            this.Id = Guid.NewGuid();
            this.IdPaciente = IdPaciente;
            this.IdMedico = IdMedico;
            this.DataInicio = DataInicio;
            this.DataFim = DataFim;
        }

        [JsonConstructor]
        public Agendamento(String Id, DateTime DataInicio, DateTime DataFim, String IdMedico, String IdPaciente)
        {
            this.Id = new Guid(Id);
            this.IdPaciente = new Guid(IdPaciente);
            this.IdMedico = new Guid(IdMedico);
            this.DataInicio = DataInicio;
            this.DataFim = DataFim;
        }
    }
}
