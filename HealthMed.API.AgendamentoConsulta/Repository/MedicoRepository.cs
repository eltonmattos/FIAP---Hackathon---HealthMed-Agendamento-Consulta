using HealthMed.API.AgendamentoConsulta.Models;
using System.Net.Mail;
using HealthMed.API.AgendamentoConsulta.Database;
using Dapper;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class MedicoRepository : UsuarioRepository
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config;

        public MedicoRepository(IConfiguration configuration)
        {
            this._config = configuration;
        }

        public Guid Post(Medico medico)
        {
            //UsuarioRepository.Validate(medico);
            //MedicoRepository.ValidateCPF(medico.CPF);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            int duracaoConsulta = medico.DuracaoConsulta != null ? (int)medico.DuracaoConsulta : this._config.GetValue<int>("DuracaoConsultaPadrao");

            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                Guid idMedico = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($"INSERT INTO {dbname}.dbo.Medico ([Id], [Nome], [CPF], [CRM], [Email], [Senha], [DuracaoConsulta]) VALUES ");
                query.Append($"(" +
                    $"'{idMedico}'," +
                    $"'{medico.Nome}'," +
                    $"'{medico.CPF}', " +
                    $"'{medico.CRM}', " +
                    $"'{medico.Email}', " +
                    $"@Hash," +
                    $" {duracaoConsulta} " +
                    $")");

                sqldb.connection.Open();
                SqlCommand command = new SqlCommand(query.ToString(), sqldb.connection);
                command.Parameters.AddWithValue("@Hash", SHA256.HashData(Encoding.UTF8.GetBytes(medico.Senha)));
                command.ExecuteNonQuery();
                sqldb.connection.Close();
                return idMedico;
            }
        }

        public static void Validate(Medico medico)
        {
            if (!String.IsNullOrEmpty(medico.CPF))
            {
                try
                {
                    ValidateCPF(medico.CPF);
                }
                catch (FormatException)
                {
                    throw new FormatException("CPF inválido");
                }
            }
        }
    }

}
