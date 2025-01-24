namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class AgendamentoRepository
    {
        public required Paciente paciente { get; set; }
        public required Medico medico { get; set; }
        public required DateTime dataConsulta { get; set; }
    }
}
