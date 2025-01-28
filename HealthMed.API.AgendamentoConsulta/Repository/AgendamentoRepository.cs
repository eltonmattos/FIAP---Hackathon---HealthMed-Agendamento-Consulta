using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;
using System.Xml.Linq;
using Dapper;
using System.Runtime.CompilerServices;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class AgendamentoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(Agendamento agendamento)
        {
            if (DateTime.Compare(agendamento.DataInicio, agendamento.DataFim) >= 0)
                throw new Exception("Data de início não pode ser menor ou igual à data fim");

            if (VerificarDisponibilidadeMedico(agendamento) || VerificarAgendamentosMedico(agendamento))
                throw new Exception("Horário não disponível");

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
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
                    '{agendamento.DataInicio.ToString("s")}',
                    '{agendamento.DataFim.ToString("s")}',
                    '{agendamento.Medico}',
                    '{agendamento.Paciente}'
                )");

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                 command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idAgendamento;
            }
        }

        public IEnumerable<Agendamento> Get(String idMedico, DateTime data)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT [Id]
                                    ,[DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                FROM [HealthMedAgendamento].[dbo].[Agendamento] ");

                query.Append($"WHERE [IdMedico] = '{idMedico}'");
                query.Append($"AND CAST(DataInicio AS DATE) = '{data.ToString("yyyy-MM-dd")}'");

                IEnumerable<Agendamento> result = sqldb.Connection.Query<Agendamento>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }
        }

        public IEnumerable<Agendamento> Get(String idMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"SELECT [Id]
                                    ,[DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                FROM [HealthMedAgendamento].[dbo].[Agendamento] ");
                query.Append($"WHERE [IdMedico] = '{idMedico}'");

                IEnumerable<Agendamento> result = sqldb.Connection.Query<Agendamento>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }
        }

        private bool VerificarDisponibilidadeMedico(Agendamento agendamento)
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(this._config);
            IEnumerable<DisponibilidadeMedico> horariosDisponiveisMedico = [];
            disponibilidadeMedicoRepository.Get(agendamento.Medico.ToString());

            int DiaSemana = (int)agendamento.DataInicio.DayOfWeek;

            foreach (DisponibilidadeMedico d in horariosDisponiveisMedico)
            {
                if (d.DiaSemana != DiaSemana)
                    continue;
                if (agendamento.DataInicio.TimeOfDay >= d.InicioPeriodo && agendamento.DataFim.TimeOfDay <= d.FimPeriodo)
                    return true;
            }

            return false;
        }

        private bool VerificarAgendamentosMedico(Agendamento agendamento)
        {
            IEnumerable<Agendamento> agendamentos = this.Get(agendamento.Medico.ToString(), agendamento.DataInicio);

            foreach (Agendamento a in agendamentos)
            {
                if (DateTime.Compare(agendamento.DataInicio, a.DataInicio) >= 0 && DateTime.Compare(agendamento.DataInicio, a.DataFim) <= 0)
                    return true;
                if (DateTime.Compare(agendamento.DataFim, a.DataInicio) >= 0 && DateTime.Compare(agendamento.DataFim, a.DataFim) <= 0)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Título do e-mail:
        /// Após o agendamento, feito pelo usuário Paciente, o médico deverá receber um e-mail contendo:
        ///        ˮHealthMed - Nova consulta agendadaˮ
        ///        Corpo do e-mail:
        ///  ˮOlá, Dr. {nome_do_médico}!
        /// Você tem uma nova consulta marcada! Paciente: {nome_do_paciente}.
        /// Data e horário: { data} às {horário_agendado}.ˮ
        /// </summary>
        /// <param name="agendamento"></param>
        public async void NotificarAgendamento(Agendamento agendamento)
        {
            PacienteRepository pacienteRepository = new(this._config);
            Paciente? paciente = pacienteRepository.Get(agendamento.Paciente.ToString());

            MedicoRepository medicoRepository = new(this._config);
            Medico? medico = medicoRepository.Get(agendamento.Medico.ToString());

            if (paciente!= null && medico != null)
            {
                MailRequest mail = new()
                {
                    ToEmail = medico.Email,
                    Subject = "Health&Med - Nova consulta agendada",
                    Body = @$"Olá, Dr. {medico.Nome}! 
                           Você tem uma nova consulta marcada! 
                           Paciente: {paciente.Nome}. 
                           Data e horário: {agendamento.DataInicio:dd/MM/yyyy} às {agendamento.DataInicio:HH:mm}"
                };
                MailService mailService = new(this._config);
                await mailService.SendEmailAsync(mail);
            }
            
        }
    }
}
