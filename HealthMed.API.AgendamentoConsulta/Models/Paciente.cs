﻿using HealthMed.API.AgendamentoConsulta.Repository;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthMed.API.AgendamentoConsulta.Models
{
    public class Paciente : Usuario
    {
        public string CPF { get; set; }

        public Paciente(String Nome, String CPF, String Email)
        {
            SetId(System.Guid.NewGuid());
            this.Nome = Nome;
            this.CPF = CPF;
            this.Email = Email;
        }

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
