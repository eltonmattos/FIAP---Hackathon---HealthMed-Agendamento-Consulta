using System.Text.Json.Serialization;

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
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        [JsonConstructor]
        public Usuario()
        {
            SetId(Guid.NewGuid());
        }
    }
}
