using HealthMed.API.AgendamentoConsulta.Repository;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Paciente : Usuario
    {
        public required string CPF { get; set; }
        [JsonConstructor]
        public Paciente(string nome, string email, string CPF, string senha)
        {
            SetId(Guid.NewGuid());
            this.Nome = nome;
            this.Email = email;
            this.Senha = senha;
            this.CPF = CPF;
        }
    }
}
