using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class DisponibilidadeMedico
    {
        public Guid Id { get; set; } 
        public required Guid IdMedico { get; set; }
        public required int DiaSemana { get; set; }
        public required TimeSpan InicioPeriodo { get; set; }
        public required TimeSpan FimPeriodo { get; set; }
        public required DateTime Validade { get; set; }

        public DisponibilidadeMedico() { }

        [SetsRequiredMembers]
        public DisponibilidadeMedico(Guid Medico, int DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade)
        {
            this.Id = Guid.NewGuid();
            this.IdMedico = Medico;
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }

        [JsonConstructor]
        public DisponibilidadeMedico(String Id, Int32 DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade, String IdMedico)
        {
            this.Id = new Guid(Id);
            this.IdMedico = new Guid(IdMedico);
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }
    }
}