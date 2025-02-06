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

        public IEnumerable<object> Post(IEnumerable<DisponibilidadeMedico> disponibilidadesMedico)
        {
            if (disponibilidadesMedico == null || !disponibilidadesMedico.Any())
                return new List<object>();


            List<object> results = [];

            dbname = this._config.GetValue<string>("DatabaseName");

            StringBuilder query = new();
            query.Append($@"INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
                            ([Id], [DiaSemana], [InicioPeriodo], [FimPeriodo], [Validade], [IdMedico]) VALUES ");

            int validados = 0;
            foreach (var d in disponibilidadesMedico)
            {
                Guid idDisponibilidadeMedico = Guid.NewGuid();
                String result = String.Empty;
                try
                {
                    ValidateDataValidade(d.Validade);
                    ValidateHorario(d.InicioPeriodo, d.FimPeriodo);
                    ValidateDisponibilidadeMedico(d.IdMedico, d.DiaSemana, d.InicioPeriodo, d.FimPeriodo);
                    {
                        query.Append($@" ('{idDisponibilidadeMedico}', {d.DiaSemana}, '{d.InicioPeriodo}', '{d.FimPeriodo}', '{d.Validade}', '{d.IdMedico}')");
                        if (d != disponibilidadesMedico.Last())
                            query.Append(", ");
                        result = "Periodo de disponibilidade cadastrado com sucesso.";
                        validados++;
                    }
                }
                catch (Exception e)
                {
                    result = e.Message;
                }

                results.Add(new { id = idDisponibilidadeMedico, message = result });
            }

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                if (validados > 0)
                {
                    sqldb.Connection.Open();

                    SqlCommand command = new(query.ToString(), sqldb.Connection);

                    command.ExecuteNonQuery();
                }
            }
            return results;

        }
        public void Put(String idMedico, String idDisponibilidadeMedico, DisponibilidadeMedico disponibilidadeMedico)
        {
            ValidateDataValidade(disponibilidadeMedico.Validade);
            ValidateHorario(disponibilidadeMedico.InicioPeriodo, disponibilidadeMedico.FimPeriodo);
            ValidateDisponibilidadeMedico(disponibilidadeMedico.IdMedico,
                                            disponibilidadeMedico.DiaSemana,
                                            disponibilidadeMedico.InicioPeriodo,
                                            disponibilidadeMedico.FimPeriodo);
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
                              FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico] ");
                query.Append($"WHERE IdMedico = '{idMedico}' ");
                query.Append($"AND CAST(InicioPeriodo AS DATE) = '{data.ToString("yyyy-MM-dd")}'");

                IEnumerable<DisponibilidadeMedico> result = sqldb.Connection.Query<DisponibilidadeMedico>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }
        }


        public IEnumerable<DisponibilidadeMedico> Get(string idMedico, int diaSemana)
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
                              FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico] ");
                query.Append($"WHERE IdMedico = '{idMedico}' ");
                query.Append($"AND [DiaSemana] = {diaSemana}");

                //IEnumerable<DisponibilidadeMedico> result = sqldb.Connection.Query<DisponibilidadeMedico>(
                //    query.ToString(), param: null);

                var result = sqldb.Connection.Query(query.ToString())
                    .Select(r => new DisponibilidadeMedico
                    {
                        DiaSemana = r.DiaSemana,
                        InicioPeriodo = r.InicioPeriodo,
                        FimPeriodo = r.FimPeriodo,
                        Validade = r.Validade,
                        IdMedico = Guid.Parse(r.IdMedico)
                    }).ToList();

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

        public void ValidateDataValidade(DateTime dataValidade)
        {
            if (dataValidade < DateTime.Now)
            {
                throw new Exception("Data de Validade não pode ser inferior a Data Atual.");
            }
        }

        public void ValidateHorario(TimeSpan inicio, TimeSpan fim)
        {
            if (inicio > fim)
            {
                throw new Exception("Horário de Início não pode ser superior ao Horário de Fim.");
            }
        }

        public void ValidateDisponibilidadeMedico(Guid idMedico, int diaSemana, TimeSpan inicioPeriodo, TimeSpan fimPeriodo)
        {
            List<DisponibilidadeMedico> DisponibilidadesMedico = Get(idMedico.ToString(), diaSemana).ToList();

            foreach (var d in DisponibilidadesMedico)
            {
                //Verificar se o agendamento a ser criado não ira conflitar com os horarios já obtidos na base
                if (d.DiaSemana == diaSemana)
                {
                    if (inicioPeriodo >= d.InicioPeriodo && fimPeriodo <= d.FimPeriodo)
                    {
                        throw new Exception("Horário de disponibilidade conflitante com outro horário já cadastrado.");
                    }
                }
            }
        }
    }
}
