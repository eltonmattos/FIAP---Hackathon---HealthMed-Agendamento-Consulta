namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class AgendamentoRepository
    {
        public required PacienteRepository paciente { get; set; }
        public required MedicoRepository medico { get; set; }
        public required DateTime dataConsulta { get; set; }
    }
}
