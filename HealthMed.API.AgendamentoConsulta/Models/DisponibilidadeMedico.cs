using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class DisponibilidadeMedico
    {
        protected Guid Id { get; set; }
        public Guid GetId()
        {
            return Id;
        }

        protected void SetId(Guid value)
        {
            Id = value;
        }
        public required Guid IdMedico { get; set; }
        public required int DiaSemana { get; set; }
        public required TimeSpan InicioPeriodo { get; set; }
        public required TimeSpan FimPeriodo { get; set; }
        public required DateTime Validade { get; set; }

        public DisponibilidadeMedico() { }

        [SetsRequiredMembers]
        public DisponibilidadeMedico(Guid Medico, int DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade)
        {
            SetId(Guid.NewGuid());
            this.IdMedico = Medico;
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }

        //[SetsRequiredMembers]
        //[JsonConstructor]
        //public DisponibilidadeMedico(Int32 DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, String Validade, String IdMedico)
        //{
        //    this.IdMedico = new Guid(IdMedico);
        //    this.DiaSemana = DiaSemana;
        //    this.InicioPeriodo = InicioPeriodo;
        //    this.FimPeriodo = FimPeriodo;
        //    this.Validade = DateTime.Parse(Validade);
        //}
    }
}