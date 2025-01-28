using HealthMed.API.AgendamentoConsulta.Repository;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Medico : Usuario
    {
        public required string CPF { get; set; }
        public required string CRM { get; set; }
        public int? DuracaoConsulta { get; set; }

        public Medico(String id, String nome)
        {
            SetId(Guid.Parse(id));
            this.Nome = nome;
        }
        [JsonConstructor]
        public Medico(string nome, string email, string CPF, string CRM, string senha, int? duracaoConsulta)
        {
            SetId(Guid.NewGuid());
            this.Nome = nome;
            this.Email = email;
            this.Senha = senha;
            this.CRM = CRM;
            this.CPF = CPF;
            this.DuracaoConsulta = duracaoConsulta;
        }
    }
}
