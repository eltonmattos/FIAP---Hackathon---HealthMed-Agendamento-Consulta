using System.Net.Mail;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public enum Funcao
    {
        Administrador,
        Medico,
        Paciente
    }
    public class Usuario
    {
        public Guid Id { get; set; }
        public Guid GetId()
        {
            return Id;
        }

        protected void SetId(Guid value)
        {
            Id = value;
        }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required Funcao Funcao { get; set; }

        public Usuario()
        {
            SetId(Guid.NewGuid());
        }
    }
}
