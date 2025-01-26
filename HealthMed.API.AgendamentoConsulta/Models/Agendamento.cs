namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Agendamento
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
        public required Guid paciente { get; set; }
        public required Guid medico { get; set; }
        public required DateTime dataInicio { get; set; }
        public required DateTime dataFim { get; set; }

        public Agendamento(Guid paciente, Guid medico, DateTime dataInicio, DateTime dataFim)
        {
            SetId(Guid.NewGuid());
            this.paciente = paciente;
            this.medico = medico;
            this.dataInicio = dataInicio;
            this.dataFim = dataFim;
        }
    }
}
