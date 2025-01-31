using System;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class DisponibilidadeMedico
    {
        public Guid Id { get; set; } 
        public required Guid Medico { get; set; }
        public required int DiaSemana { get; set; }
        public required TimeSpan InicioPeriodo { get; set; }
        public required TimeSpan FimPeriodo { get; set; }
        public required DateTime Validade { get; set; }

        public DisponibilidadeMedico(Guid Medico, int DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade)
        {
            this.Id = Guid.NewGuid();
            this.Medico = Medico;
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }

        [JsonConstructor]
        public DisponibilidadeMedico(Guid Id, Guid Medico, int DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade)
        {
            this.Id = Id;
            this.Medico = Medico;
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }
    }
}