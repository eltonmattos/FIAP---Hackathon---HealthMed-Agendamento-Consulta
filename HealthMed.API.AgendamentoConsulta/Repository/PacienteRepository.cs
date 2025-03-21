﻿using Dapper;
using HealthMed.API.AgendamentoConsulta.Services;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class PacienteRepository(IConfiguration configuration) : UsuarioRepository
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(Paciente paciente)
        {
            if (paciente.Email == null || paciente.CPF == null || paciente.Senha == null)
                throw new Exception("Dados inválidos");
            UsuarioRepository.ValidateEmail(paciente.Email);
            UsuarioRepository.ValidateCPF(paciente.CPF);
            UsuarioRepository.ValidatePassword(paciente.Senha);
            ValidatePacienteExiste(paciente.Email, paciente.CPF);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                Guid idPaciente = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($"INSERT INTO {dbname}.dbo.Paciente ([Id], [Nome], [CPF], [Email], [Senha]) VALUES ");
                query.Append($"(" +
                    $"'{idPaciente}'," +
                    $"'{paciente.Nome}'," +
                    $"'{paciente.CPF}', " +
                    $"'{paciente.Email}', " +
                    $"@Hash" +
                    $")");

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                command.Parameters.AddWithValue("@Hash", SHA256.HashData(Encoding.UTF8.GetBytes(paciente.Senha)));
                command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idPaciente;
            }
        }
        public Paciente? Get(String idPaciente)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT [Nome],[CPF],[Email]
                              FROM [HealthMedAgendamento].[dbo].[Paciente]
                              WHERE [Id] = '{idPaciente}' ");

                IEnumerable<Paciente> result = sqldb.Connection.Query<Paciente>(query.ToString(), param: null);

                sqldb.Connection.Close();
                
                return result.FirstOrDefault();
            }
        }


        public void Delete(String email)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"DELETE FROM [HealthMedAgendamento].[dbo].[Paciente] WHERE [Email] = '{email}' ");

                sqldb.Connection.Query<Paciente>(query.ToString(), param: null);

                sqldb.Connection.Close();
            }
        }

        public string? GetToken(string email, string senha, bool isMedico)
        {
            UsuarioRepository.ValidateEmail(email);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");

                // Define a tabela conforme o tipo de usuário
                string tableName = isMedico ? "Medico" : "Paciente";

                query.Append($"SELECT [Id] FROM {dbname}.dbo.{tableName} ");
                query.Append($"WHERE [Email] = '{email}' AND [Senha] = HASHBYTES('SHA2_256', '{senha}')");

                string? userId = sqldb.Connection?.QueryFirstOrDefault<string?>(query.ToString());

                if (!string.IsNullOrEmpty(userId))
                {
                    // Define a role com base no tipo de usuário
                    string role = isMedico ? "Medico" : "Paciente";

                    var jwtService = new JwtService(_config);
                    return jwtService.GenerateToken(email, role);
                }

                return null;
            }
        }


        /// <summary>
        /// Valida se o paciente existe no banco de dados, a partir do email + CPF
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cpf"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void ValidatePacienteExiste(String email, String cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                Guid idPaciente = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($"SELECT [Id] FROM {dbname}.dbo.Paciente ");
                query.Append($"WHERE [Email] = '{email}' OR [CPF] = '{cpf}'");

                String? result = sqldb.Connection?.QueryFirstOrDefault<String>(query.ToString());

                if (result != null)
                    throw new Exception("Paciente já cadastrado");
            }
        }
    }

    public class HorarioDisponivelAgendamento
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
