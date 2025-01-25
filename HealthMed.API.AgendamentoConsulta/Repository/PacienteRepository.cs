using Dapper;
using HealthMed.API.AgendamentoConsulta.Database;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class PacienteRepository: UsuarioRepository
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config;

        public PacienteRepository(IConfiguration configuration)
        {
            this._config = configuration;
        }

        public Guid Post(Paciente paciente)
        {
            UsuarioRepository.ValidateEmail(paciente.Email);
            UsuarioRepository.ValidateCPF(paciente.CPF);
            UsuarioRepository.ValidatePassword(paciente.Senha);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
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

                sqldb.connection.Open();
                SqlCommand command = new SqlCommand(query.ToString(), sqldb.connection);
                command.Parameters.AddWithValue("@Hash", SHA256.HashData(Encoding.UTF8.GetBytes(paciente.Senha)));
                command.ExecuteNonQuery();
                sqldb.connection.Close();
                return idPaciente;
            }
        }

        public String GetToken(String email, String senha)
        {
            UsuarioRepository.ValidateEmail(email);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                Guid idPaciente = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($"SELECT [Id] FROM {dbname}.dbo.Paciente ");
                query.Append($"WHERE [Email] = '{email}' AND [Senha] = HASHBYTES('SHA2_256', '{senha}')");

                String? result = sqldb.connection?.QueryFirstOrDefault<String>(query.ToString());

                if (result == null)
                    throw new Exception("Usuário não encontrado");

                return result.ToString();
            }
        }

        //TODO: Validar se usuario já existe (Email + CPF)
    }
}
