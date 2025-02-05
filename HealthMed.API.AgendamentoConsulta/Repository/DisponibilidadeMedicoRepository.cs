using Dapper;
using HealthMed.API.AgendamentoConsulta.Services;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.Identity.Client;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class DisponibilidadeMedicoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public IEnumerable<Guid> Post(IEnumerable<DisponibilidadeMedico> disponibilidadesMedico)
        {
            if (disponibilidadesMedico == null || disponibilidadesMedico.Count() == 0)
                return new List<Guid>();

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                List<Guid> ids = new List<Guid>();

                Guid idDisponibilidadeMedico = Guid.NewGuid();

                dbname = this._config.GetValue<string>("DatabaseName");

                StringBuilder query = new StringBuilder();
                query.Append($@"INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
                                ([Id], [DiaSemana], [InicioPeriodo], [FimPeriodo], [Validade], [IdMedico]) VALUES ");

                foreach (var d in disponibilidadesMedico)
                {
                    try
                    {
                        ValidadeDataValidade(d.Validade);
                        ValidadeHorario(d.InicioPeriodo, d.FimPeriodo);
                        query.Append($@"VALUES 
                                        ('{Guid.NewGuid()}', {d.DiaSemana}, '{d.InicioPeriodo}', '{d.FimPeriodo}', '{d.Validade}', '{d.IdMedico}')");
                        if (d != disponibilidadesMedico.Last())
                            query.Append(", ");
                        ids.Add(idDisponibilidadeMedico);

                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);

                command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return ids;
            }
           
        }
        public void Put(String idMedico, String idDisponibilidadeMedico, DisponibilidadeMedico disponibilidadeMedico)
        {
            ValidadeDataValidade(disponibilidadeMedico.Validade);
            ValidadeHorario(disponibilidadeMedico.InicioPeriodo, disponibilidadeMedico.FimPeriodo);

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"UPDATE {dbname}.dbo.DisponibilidadeMedico SET
                    [DiaSemana] = {disponibilidadeMedico.DiaSemana},
                    [InicioPeriodo] = '{disponibilidadeMedico.InicioPeriodo}',
                    [FimPeriodo] = '{disponibilidadeMedico.FimPeriodo}',
                    [Validade] = '{disponibilidadeMedico.Validade.Date.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                query.Append($"WHERE IdMedico = '{idMedico}' ");
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
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"SELECT [Id]
                  ,[DiaSemana]
                  ,[InicioPeriodo]
                  ,[FimPeriodo]
                  ,[Validade]
                  ,[IdMedico]
                FROM [{dbname}].[dbo].[DisponibilidadeMedico]");
                query.Append(" WHERE IdMedico = @IdMedico");

                var result = sqldb.Connection.Query(query.ToString(), new { IdMedico = idMedico.ToString() })
                    .Select(r => new DisponibilidadeMedico
                    {
                        //Id = Guid.Parse(r.Id),
                        DiaSemana = r.DiaSemana,
                        InicioPeriodo = r.InicioPeriodo,
                        FimPeriodo = r.FimPeriodo,
                        Validade = r.Validade,
                        IdMedico = Guid.Parse(r.IdMedico)
                    }).ToList();

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

        public void Delete(Guid idDisponibilidade)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"DELETE FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico] WHERE [id] = '{idDisponibilidade.ToString()}' ");
                sqldb.Connection.Query<Paciente>(query.ToString(), param: null);
                sqldb.Connection.Close();
            }
        }

        public void ValidadeDataValidade(DateTime dataValidade)
        {
            if (dataValidade < DateTime.Now)
            {
                throw new Exception("Data de Validade não pode ser inferior a Data Atual.");
            }
        }

        public void ValidadeHorario(TimeSpan inicio, TimeSpan fim)
        {
            if (inicio > fim)
            {
                throw new Exception("Horário de Início não pode ser superior ao Horário de Fim.");
            }
        }
    }
}
