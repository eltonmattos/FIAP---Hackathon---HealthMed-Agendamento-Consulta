using HealthMed.API.AgendamentoConsulta.Database;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;
using System.Xml.Linq;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class AgendamentoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(Agendamento agendamento)
        {
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
                    '{agendamento.dataInicio}',
                    `{agendamento.dataFim}',
                    '{agendamento.medico}',
                    '{agendamento.paciente}' 
                );
                ");

                sqldb.connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.connection);
                 command.ExecuteNonQuery();
                sqldb.connection.Close();
                return idAgendamento;
            }
        }

        public List<Agendamento> Get(DateTime data)
        {
            List<Agendamento> agendamentos = [];

            //sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            //if (sqldb == null || sqldb.connection == null)
            //    throw new Exception("SQL ERROR");

            //using (sqldb.connection)
            //{
            //    Guid idAgendamento = Guid.NewGuid();
            //    var query = new StringBuilder();
            //    dbname = this._config.GetValue<string>("DatabaseName");
            //    query.Append($@"INSERT INTO [dbo].[Agendamento] (
            //        [Id], 
            //        [DataInicio], 
            //        [DataFim], 
            //        [IdMedico], 
            //        [IdPaciente]
            //    ) VALUES (
            //        '{idAgendamento}',
            //        '{agendamento.dataInicio}',
            //        `{agendamento.dataFim}',
            //        '{agendamento.medico}',
            //        '{agendamento.paciente}' 
            //    );
            //    ");

            //    sqldb.connection.Open();
            //    SqlCommand command = new SqlCommand(query.ToString(), sqldb.connection);
            //    command.ExecuteNonQuery();
            //    sqldb.connection.Close();
            //    return idAgendamento;
            //}
            return agendamentos;
        }
        public List<Agendamento> Get()
        {
            List<Agendamento> agendamentos = [];
            //sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            //if (sqldb == null || sqldb.connection == null)
            //    throw new Exception("SQL ERROR");

            //using (sqldb.connection)
            //{
            //    Guid idAgendamento = Guid.NewGuid();
            //    var query = new StringBuilder();
            //    dbname = this._config.GetValue<string>("DatabaseName");
            //    query.Append($@"INSERT INTO [dbo].[Agendamento] (
            //        [Id], 
            //        [DataInicio], 
            //        [DataFim], 
            //        [IdMedico], 
            //        [IdPaciente]
            //    ) VALUES (
            //        '{idAgendamento}',
            //        '{agendamento.dataInicio}',
            //        `{agendamento.dataFim}',
            //        '{agendamento.medico}',
            //        '{agendamento.paciente}' 
            //    );
            //    ");

            //    sqldb.connection.Open();
            //    SqlCommand command = new SqlCommand(query.ToString(), sqldb.connection);
            //    command.ExecuteNonQuery();
            //    sqldb.connection.Close();
            //    return idAgendamento;
            //}

            return agendamentos;
        }


    }
}
