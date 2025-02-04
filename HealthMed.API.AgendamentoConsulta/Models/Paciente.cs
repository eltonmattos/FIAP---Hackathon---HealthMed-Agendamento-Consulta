using HealthMed.API.AgendamentoConsulta.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Paciente : Usuario
    {
        public required string CPF { get; set; }

        [SetsRequiredMembers]
        public Paciente(String Nome, String CPF, String Email)
        {
            SetId(System.Guid.NewGuid());
            this.Nome = Nome;
            this.CPF = CPF;
            this.Email = Email;
        }

        [SetsRequiredMembers]
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
