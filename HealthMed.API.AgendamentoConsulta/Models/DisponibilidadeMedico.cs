using Microsoft.AspNetCore.Http.HttpResults;

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
        public required Guid Medico { get; set; }
        public required int DiaSemana { get; set; }
        public required TimeSpan InicioPeriodo { get; set; }
        public required TimeSpan FimPeriodo { get; set; }
        public required DateTime Validade { get; set; }

        public DisponibilidadeMedico(String Id, Int32 DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade, String IdMedico)
        {
            this.Id = Guid.Parse(Id);
            this.Medico = Guid.Parse(IdMedico);
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }

        public DisponibilidadeMedico(Guid idMedico, Int32 DiaSemana, TimeSpan InicioPeriodo, TimeSpan FimPeriodo, DateTime Validade)
        {
            SetId(Guid.NewGuid());
            this.Medico = idMedico;
            this.DiaSemana = DiaSemana;
            this.InicioPeriodo = InicioPeriodo;
            this.FimPeriodo = FimPeriodo;
            this.Validade = Validade;
        }
    }
}
