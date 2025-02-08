using HealthMed.API.AgendamentoConsulta.Models;
using System.Net.Mail;
using HealthMed.API.AgendamentoConsulta.Services;
using Dapper;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class MedicoRepository(IConfiguration configuration) : UsuarioRepository
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(Medico medico)
        {
            ArgumentNullException.ThrowIfNull(medico);
            if (String.IsNullOrEmpty(medico.Email))
                throw new ArgumentException("Nome não pode ser nulo");
            if (String.IsNullOrEmpty(medico.CPF))  
                throw new ArgumentException("CPF não pode ser nulo");
            if (String.IsNullOrEmpty(medico.CRM))
                throw new ArgumentException("CRM não pode ser nulo");
            if (String.IsNullOrEmpty(medico.Senha))
                throw new ArgumentException("Senha não pode ser nula");

            new MedicoRepository(_config).ValidateCRM(medico.CRM);

            UsuarioRepository.ValidateEmail(medico.Email);
            UsuarioRepository.ValidateCPF(medico.CPF);
            UsuarioRepository.ValidatePassword(medico.Senha);
            ValidateMedicoExiste(medico.Email, medico.CPF, medico.CRM);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            int duracaoConsulta = medico.DuracaoConsulta != null ? (int)medico.DuracaoConsulta : this._config.GetValue<int>("DuracaoConsultaPadrao");

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
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

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                command.Parameters.AddWithValue("@Hash", SHA256.HashData(Encoding.UTF8.GetBytes(medico.Senha)));
                command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idMedico;
            }
        }

        public IEnumerable<object> GetMedicos(String? especialidade = "", String? estado = "", String? crm = "")
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                bool whereUsed = false;
                var query = new StringBuilder();
                query.Append(@$"SELECT 
                                    m.Id AS IdMedico,
                                    m.Nome AS NomeMedico,
                                    m.CRM AS CRMMedico,
                                    e.Id AS IdEspecialidade,
                                    e.Nome AS NomeEspecialidade,
                                    m.ValorConsulta AS ValorConsulta
                                FROM 
                                    dbo.Medico m
                                LEFT JOIN 
                                    dbo.rel_Especialidades_Medico re ON m.Id = re.IdMedico
                                LEFT JOIN 
                                    dbo.Especialidades e ON re.IdEspecialidade = e.Id ");

                if (!String.IsNullOrEmpty(especialidade))
                {
                    query.Append("WHERE "); whereUsed = true;
                    query.Append($"UPPER(e.Nome) LIKE '%{especialidade.ToUpper()}%' ");
                }
                if (!String.IsNullOrEmpty(estado))
                {
                    if (!whereUsed) {
                        query.Append("WHERE "); whereUsed = true;
                    }
                    else
                    {
                        query.Append("AND "); 
                    }
                    query.Append($" m.CRM LIKE '%{estado}%' ");
                }
                if (!String.IsNullOrEmpty(crm))
                {
                    if (!whereUsed)
                    {
                        query.Append("WHERE "); 
                    }
                    else
                    {
                        query.Append("AND ");
                    }
                    query.Append($"m.CRM = '{crm}' ");
                }

                IEnumerable<object> result = sqldb.Connection.Query<object>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();

                return result;
            }
        }

        public Medico? Get(String idMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT [Nome],[Email],[CPF],[CRM],[DuracaoConsulta],[ValorConsulta]
                              FROM [HealthMedAgendamento].[dbo].[Medico] WHERE [Id] = '{idMedico}' ");

                IEnumerable<Medico> result = sqldb.Connection.Query<Medico>(query.ToString(), param: null);

                sqldb.Connection.Close();

                return result.FirstOrDefault();
            }
        }

        public Guid HorarioDisponivel(Medico medico)
        {
            ArgumentNullException.ThrowIfNull(medico);
            if (String.IsNullOrEmpty(medico.Email))
                throw new ArgumentException("Nome não pode ser nulo");
            if (String.IsNullOrEmpty(medico.CPF))
                throw new ArgumentException("CPF não pode ser nulo");
            if (String.IsNullOrEmpty(medico.CRM))
                throw new ArgumentException("CRM não pode ser nulo");
            if (String.IsNullOrEmpty(medico.Senha))
                throw new ArgumentException("Senha não pode ser nula");
            UsuarioRepository.ValidateEmail(medico.Email);
            UsuarioRepository.ValidateCPF(medico.CPF);
            UsuarioRepository.ValidatePassword(medico.Senha);
            ValidateMedicoExiste(medico.Email, medico.CPF, medico.CRM);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            int duracaoConsulta = medico.DuracaoConsulta != null ? (int)medico.DuracaoConsulta : this._config.GetValue<int>("DuracaoConsultaPadrao");

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
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

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                command.Parameters.AddWithValue("@Hash", SHA256.HashData(Encoding.UTF8.GetBytes(medico.Senha)));
                command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idMedico;
            }
        }

        public string? GetToken(string crm, string senha, bool isMedico)
        {
            new MedicoRepository(_config).ValidateCRM(crm);

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
                query.Append($"WHERE [CRM] = '{crm}' AND [Senha] = HASHBYTES('SHA2_256', '{senha}')");

                string? userId = sqldb.Connection?.QueryFirstOrDefault<string?>(query.ToString());

                if (!string.IsNullOrEmpty(userId))
                {
                    // Define a role com base no tipo de usuário
                    string role = isMedico ? "Medico" : "Paciente";

                    var jwtService = new JwtService(_config);
                    return jwtService.GenerateToken(crm, role);
                }

                return null;
            }
        }


        /// <summary>
        /// Valida se o médico existe no banco de dados, a partir do email + CPF
        /// Não é verificado o CRM, pois um médico pode ter mais de um CRM, para estados diferentes
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cpf"></param>
        /// <param name="crm"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void ValidateMedicoExiste(String email, String cpf, String crm)
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
                query.Append($"SELECT [Id] FROM {dbname}.dbo.Medico ");
                query.Append($"WHERE [Email] = '{email}' OR [CPF] = '{cpf}'  OR [CRM] = '{crm}'");

                String? result = sqldb.Connection?.QueryFirstOrDefault<String>(query.ToString());

                if (result != null)
                    throw new Exception("Médico já cadastrado");
            }
        }

        public void ValidateCRM(String crm)
        {
            string padrao = @"^\d{6}(-P)?-[A-Z]{2}$";
            if (!Regex.IsMatch(crm, padrao))
                throw new FormatException("CRM inválido");
        }

        public void Delete(String email)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"DELETE FROM [HealthMedAgendamento].[dbo].[Medico] WHERE [Email] = '{email}' ");
                sqldb.Connection.Query<Paciente>(query.ToString(), param: null);
                sqldb.Connection.Close();
            }
        }
    }
}
