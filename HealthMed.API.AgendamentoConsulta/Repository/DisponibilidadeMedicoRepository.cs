using Dapper;
using HealthMed.API.AgendamentoConsulta.Services;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.Identity.Client;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;

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
                return [];


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
                        query.Append($@" ('{idDisponibilidadeMedico}', {d.DiaSemana}, '{d.InicioPeriodo}', '{d.FimPeriodo}', '{d.Validade.ToString("s")}', '{d.IdMedico}')");
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
            if(!ValidateSeExisteDisponibilidadeMedico(idDisponibilidadeMedico))
            {
                throw new Exception("Dispobilidade não encontrada");
            }
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

        public string AIPost(string userPrompt, string idMedico)
        {
            String? apiKey = _config.GetValue<String>("AIService:GeminiAPIKEY");
            String? apiUrl = _config.GetValue<String>("AIService:GeminiBaseURL");
            if (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(apiUrl))
                throw new Exception("API Key ou URL não configuradas.");

            AIService aiService = new(apiKey, apiUrl);

            String prompt = @$"
Você deverá ler o prompt de um usuário, que deverá seguir um determinado conjunto de regras:
- O usuario irá informar uma agenda de disponibilidade. Será solicitado que você crie uma agenda, onde ele irá informar
o dia da semana, o horário de início e fim do período de disponibilidade e a validade da agenda. 

Você deve responder em um JSon valido neste formato:
[
  {{
    ""IdMedico"": ""{idMedico}"",
    ""DiaSemana"": 0,
    ""InicioPeriodo"": ""string"",
    ""FimPeriodo"": ""string"",
    ""Validade"": ""2025-02-06T12:27:26.655Z""
  }}
]

- O json será direcionado para uma requisição http. Não insira informações adicionais que possam invalidar a requisição
- diaSemana segue o formato System.DayOfWeek do .NET, onde 0 é Domingo, 1 é Segunda, 2 é Terça, 3 é Quarta, 4 é Quinta, 5 é Sexta e 6 é Sábado.
- inicioPeriodo e fimPeriodo são horários no formato HH:mm:ss

Caso o usuário não forneca as informações corretamente e não siga as regras, você deverá retornar uma mensagem de erro informando o que está errado ao invés do JSON.

- Hoje é {DateTime.Now.ToLongDateString()} 
[PROMPT]{userPrompt}[/PROMPT]";

            String resultado = aiService.PromptApi(prompt).Result;

            JObject jsonObject = JObject.Parse(resultado);

            // Extrair o texto
            string? texto = (jsonObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()) ?? throw new Exception("Invalid response from AI service.");

            texto = texto.Replace("```json","").Replace("```", "");

            try
            {
                DeleteAgenda(Guid.Parse(idMedico));
                List<DisponibilidadeMedico>? disponibilidades = JsonSerializer.Deserialize<List<DisponibilidadeMedico>>(texto);
                DisponibilidadeMedicoRepository disponibilidadeMedico = new(_config);
                if (disponibilidades!= null)
                    if (disponibilidades.Count != 0)
                        disponibilidadeMedico.Post(disponibilidades);
                return texto;
            }
            catch (Exception e)
            {
                return e.Message;
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

        public void DeleteAgenda(Guid idMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"DELETE FROM [HealthMedAgendamento].[dbo].[DisponibilidadeMedico] WHERE [IdMedico] = '{idMedico.ToString()}' ");
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

        public bool ValidateSeExisteDisponibilidadeMedico(string idDisponibilidade)
        {
            return GetByIdDisponibilidade(idDisponibilidade.ToString());
        }

        public bool GetByIdDisponibilidade(string idDisponibilidade)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"SELECT 1
                FROM [{dbname}].[dbo].[DisponibilidadeMedico]");
                query.Append(" WHERE Id = @IdDisp");

                var result = sqldb.Connection.Query(query.ToString(), new { IdDisp = idDisponibilidade.ToString() });
                    
                return result.Any();
            }
        }

    }
}
