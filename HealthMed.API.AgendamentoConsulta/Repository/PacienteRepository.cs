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
            //UsuarioRepository.Validate(paciente);
            //PacienteRepository.ValidateCPF(paciente.CPF);

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


        public static void Validate(Paciente paciente)
        {
            if (!String.IsNullOrEmpty(paciente.CPF))
            {
                try
                {
                    ValidateCPF(paciente.CPF);
                }
                catch (FormatException)
                {
                    throw new FormatException("CPF inválido");
                }
            }
        }
    }
}
