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

        public required Guid Paciente { get; set; }
        public required Guid Medico { get; set; }
        public required DateTime DataInicio { get; set; }
        public required DateTime DataFim { get; set; }
        public Agendamento(String Id, DateTime DataInicio, DateTime DataFim, String IdMedico, String IdPaciente)
        {
            this.Id = Guid.Parse(Id);
            this.Paciente = Guid.Parse(IdPaciente);
            this.Medico = Guid.Parse(IdMedico);
            this.DataInicio = DataInicio;
            this.DataFim = DataFim;
        }
        public Agendamento(Guid paciente, Guid medico, DateTime dataInicio, DateTime dataFim)
        {
            SetId(Guid.NewGuid());
            this.Paciente = paciente;
            this.Medico = medico;
            this.DataInicio = dataInicio;
            this.DataFim = dataFim;
        }
    }
}
