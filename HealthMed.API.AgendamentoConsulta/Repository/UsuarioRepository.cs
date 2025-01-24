namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public enum Funcao
    {
        Medico,
        Paciente,
        Administrador
    }
    public class Usuario
    {
        public Guid idUsuario { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required Funcao Funcao { get; set; }
    }
}
