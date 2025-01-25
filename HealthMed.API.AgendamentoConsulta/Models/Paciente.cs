using HealthMed.API.AgendamentoConsulta.Repository;
using System.Security.Cryptography;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Paciente : Usuario
    {
        public required string CPF { get; set; }

        public Paciente(string nome, string email, string CPF, string senha)
        {
            SetId(Guid.NewGuid());
            this.Nome = nome;
            this.Funcao = Funcao.Paciente;
            this.Email = email;
            this.Senha = senha;
            this.CPF = CPF;

            PacienteRepository.Validate(this);
        }
    }
}
