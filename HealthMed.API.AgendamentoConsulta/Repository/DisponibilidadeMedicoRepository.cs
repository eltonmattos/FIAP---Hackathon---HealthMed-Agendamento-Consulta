using Dapper;
using HealthMed.API.AgendamentoConsulta.Services;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class DisponibilidadeMedicoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(DisponibilidadeMedico disponibilidadeMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                Guid idDisponibilidadeMedico = Guid.NewGuid();
                dbname = this._config.GetValue<string>("DatabaseName");

                string query = $@"
                                    INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
                                        ([Id], [DiaSemana], [InicioPeriodo], [FimPeriodo], [Validade], [IdMedico]) 
                                    VALUES 
                                        (@Id, @DiaSemana, @InicioPeriodo, @FimPeriodo, @Validade, @IdMedico)";

                sqldb.Connection.Open();
                SqlCommand command = new(query, sqldb.Connection);

                command.Parameters.AddWithValue("@Id", idDisponibilidadeMedico);
                command.Parameters.AddWithValue("@DiaSemana", disponibilidadeMedico.DiaSemana);
                command.Parameters.AddWithValue("@InicioPeriodo", disponibilidadeMedico.InicioPeriodo);
                command.Parameters.AddWithValue("@FimPeriodo", disponibilidadeMedico.FimPeriodo);
                command.Parameters.AddWithValue("@Validade", disponibilidadeMedico.Validade.ToString("yyyy-MM-dd HH:mm:ss")); 
                command.Parameters.AddWithValue("@IdMedico", disponibilidadeMedico.Medico);

                command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idDisponibilidadeMedico;
            }
        }
        public void Put(String idMedico, String idDisponibilidadeMedico, DisponibilidadeMedico disponibilidadeMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
                            ([Id],
                            [DiaSemana],
                            [InicioPeriodo],
                            [FimPeriodo],
                            [Validade],
                            [IdMedico]) 
                            VALUES (
                            '{idDisponibilidadeMedico}',
                            {disponibilidadeMedico.DiaSemana},
                            '{disponibilidadeMedico.InicioPeriodo}', 
                            '{disponibilidadeMedico.FimPeriodo}', 
                            '{disponibilidadeMedico.Validade.Date.ToString()}', 
                            '{disponibilidadeMedico.Medico}'
                            )");
                query.Append($"WHERE IdMedico = '{idMedico}'");
                query.Append($"AND Id = '{idDisponibilidadeMedico}'");

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                command.ExecuteNonQuery();
                sqldb.Connection.Close();
            }
        }


        public IEnumerable<DisponibilidadeMedico> Get(string idMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                Guid idDisponibilidadeMedico = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"SELECT [Id]
                                  ,[DiaSemana]
                                  ,[InicioPeriodo]
                                  ,[FimPeriodo]
                                  ,[Validade]
                                  ,[IdMedico]
                              FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico]");
                query.Append($"WHERE IdMedico = '{idMedico}'");

                IEnumerable<DisponibilidadeMedico> result = sqldb.Connection.Query<DisponibilidadeMedico>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }
        }

        public IEnumerable<DisponibilidadeMedico> Get(string idMedico, DateTime data)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"SELECT [Id]
                                  ,[DiaSemana]
                                  ,[InicioPeriodo]
                                  ,[FimPeriodo]
                                  ,[Validade]
                                  ,[IdMedico]
                              FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico]");
                query.Append($"WHERE IdMedico = '{idMedico}'");
                query.Append($"AND CAST(InicioPeriodo AS DATE) = '{data.ToString("yyyy-MM-dd")}'");

                IEnumerable<DisponibilidadeMedico> result = sqldb.Connection.Query<DisponibilidadeMedico>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }

        }
    }
}
