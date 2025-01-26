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

        public DisponibilidadeMedico(Guid medico, int diaSemana, TimeSpan inicioPeriodo, TimeSpan fimPeriodo, DateTime validade)
        {
            SetId(Guid.NewGuid());
            this.Medico = medico;
            this.DiaSemana = diaSemana;
            this.InicioPeriodo = fimPeriodo;
            this.FimPeriodo = fimPeriodo;
            this.Validade = validade;
        }
    }
}
