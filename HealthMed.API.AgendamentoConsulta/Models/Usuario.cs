namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Usuario
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
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }

        public Usuario()
        {
            SetId(Guid.NewGuid());
        }
    }
}
