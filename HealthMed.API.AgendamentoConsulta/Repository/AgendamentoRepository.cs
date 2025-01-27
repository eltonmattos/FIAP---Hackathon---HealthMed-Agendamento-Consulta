using HealthMed.API.AgendamentoConsulta.Database;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;
using System.Xml.Linq;
using Dapper;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class AgendamentoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(Agendamento agendamento)
        {
            if (DateTime.Compare(agendamento.dataInicio, agendamento.dataFim) >= 0)
                throw new Exception("Data de início não pode ser menor ou igual à data fim");

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                Guid idAgendamento = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"INSERT INTO {dbname}.[dbo].[Agendamento] (
                    [Id], 
                    [DataInicio], 
                    [DataFim], 
                    [IdMedico], 
                    [IdPaciente]
                ) VALUES (
                    '{idAgendamento}',
                    '{agendamento.dataInicio.ToString("s")}',
                    '{agendamento.dataFim.ToString("s")}',
                    '{agendamento.medico}',
                    '{agendamento.paciente}'
                )");

                sqldb.connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.connection);
                 command.ExecuteNonQuery();
                sqldb.connection.Close();
                return idAgendamento;
            }
        }

        public IEnumerable<Agendamento> Get()
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT TOP (1000) [Id]
                                    ,[DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                FROM [HealthMedAgendamento].[dbo].[Agendamento] ");

                IEnumerable<Agendamento> result = sqldb.connection.Query<Agendamento>(
                    query.ToString(), param: null);

                sqldb.connection.Close();
                return result;
            }
        }

        public IEnumerable<Agendamento> Get(DateTime data)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT [Id]
                                    ,[DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                FROM [HealthMedAgendamento].[dbo].[Agendamento] ");

                query.Append($"WHERE DataInicio.Date = '{data.ToString("yyyy-MM-dd")}'");

                IEnumerable<Agendamento> result = sqldb.connection.Query<Agendamento>(
                    query.ToString(), param: null);

                sqldb.connection.Close();
                return result;
            }
        }
    }
}
