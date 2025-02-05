using HealthMed.API.AgendamentoConsulta.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Medico : Usuario
    {
        public required string? CPF { get; set; }
        public required string? CRM { get; set; }
        public required int? DuracaoConsulta { get; set; }
        public required decimal ValorConsulta { get; set; }

        public Medico(String id, String nome)
        {
            SetId(Guid.Parse(id));
            this.Nome = nome;
        }

        [SetsRequiredMembers]
        public Medico(String Nome, String Email, String CPF, String CRM, Int32 DuracaoConsulta, Decimal valorConsulta)
        {
            SetId(Guid.NewGuid());
            this.Nome = Nome;
            this.CPF = CPF;
            this.CRM = CRM;
            this.Senha = Senha;
            this.Email = Email;
            this.DuracaoConsulta = DuracaoConsulta;
            this.ValorConsulta = valorConsulta;
        }

        [SetsRequiredMembers]
        [JsonConstructor]
        public Medico(string nome, string email, string CPF, string CRM, string senha, int? duracaoConsulta, Decimal valorConsulta)
        {
            SetId(Guid.NewGuid());
            this.Nome = nome;
            this.Email = email;
            this.Senha = senha;
            this.CRM = CRM;
            this.CPF = CPF;
            this.DuracaoConsulta = duracaoConsulta;
            this.ValorConsulta = valorConsulta;
        }
    }
}
