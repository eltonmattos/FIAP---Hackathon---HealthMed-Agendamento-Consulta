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

            var medico = new MedicoRepository(this._config).Get(agendamento.IdMedico.ToString());
            
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
                    [IdPaciente],
                    [ValorConsulta],
                    [Status]    
                ) VALUES (
                    '{idAgendamento}',
                    '{agendamento.DataInicio.ToString("s")}',
                    '{agendamento.DataFim.ToString("s")}',
                    '{agendamento.IdMedico}',
                    '{agendamento.IdPaciente}',
                    {medico.ValorConsulta},
                    {(Int32)StatusAgendamento.Solicitado}
                )");

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                 command.ExecuteNonQuery();
                sqldb.Connection.Close();
                return idAgendamento;
            }
        }

        public Agendamento GetAgendamento(String idAgendamento)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append(@$"SELECT 
                                    [DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                    ,[Status]
                                FROM [{dbname}].[dbo].[Agendamento] ");
                query.Append($"WHERE [Id] = '{idAgendamento}' ");

                var result = sqldb.Connection.Query(query.ToString(), new { idAgendamento = idAgendamento.ToString() })
                    .Select(r => new Agendamento
                    {
                        DataFim = r.DataFim,
                        DataInicio = r.DataInicio,
                        IdMedico = Guid.Parse(r.IdMedico),
                        IdPaciente = Guid.Parse(r.IdPaciente),
                        Status = (Int32)r.Status
                    }).ToList().FirstOrDefault();

                sqldb.Connection.Close();
                return result;
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
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append(@$"SELECT 
                                    [DataInicio]
                                    ,[DataFim]
                                    ,[IdMedico]
                                    ,[IdPaciente]
                                    ,[Status]
                                FROM [{dbname}].[dbo].[Agendamento] ");

                query.Append($"WHERE [IdMedico] = '{idMedico}' ");
                query.Append($"AND CAST(DataInicio AS DATE) = '{data.ToString("yyyy-MM-dd")}' ");
                query.Append($"AND STATUS IN (0,1)");

                var result = sqldb.Connection.Query(query.ToString(), new { IdMedico = idMedico.ToString() })
                    .Select(r => new Agendamento
                    {
                        DataFim = r.DataFim,
                        DataInicio = r.DataInicio,
                        IdMedico = Guid.Parse(r.IdMedico),
                        IdPaciente = Guid.Parse(r.IdPaciente),
                        Status = (Int32)r.Status
                    }).ToList();

                sqldb.Connection.Close();
                return result;
            }
        }

        public void AlterarStatusAgendamento(Guid idAgendamento, Int32 status, String motivo="")
        {
            Agendamento agendamento = GetAgendamento(idAgendamento.ToString());

            if (agendamento == null)
                throw new Exception("Agendamento não encontrado");

            if (agendamento.Status == (int)StatusAgendamento.CanceladoPeloPaciente ||
                agendamento.Status == (int)StatusAgendamento.RecusadoPeloMedico)
                throw new Exception("Agendamento não pode ser alterado");

            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"UPDATE {dbname}.dbo.Agendamento SET
                    [Status] = {status} ");

                if (!String.IsNullOrEmpty(motivo))
                    query.Append($@", [MotivoCancelamento] = '{motivo}' ");

                query.Append($"WHERE Id = '{idAgendamento}'");

                sqldb.Connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.Connection);
                command.ExecuteNonQuery();
                sqldb.Connection.Close();
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
                                    ,[Status]
                                FROM [HealthMedAgendamento].[dbo].[Agendamento] ");
                query.Append($"WHERE [IdMedico] = '{idMedico}' ");
                query.Append($"AND STATUS IN (0,1)");

                IEnumerable<Agendamento> result = sqldb.Connection.Query<Agendamento>(
                    query.ToString(), param: null);

                sqldb.Connection.Close();
                return result;
            }
        }

        public void Delete(Guid idAgendamento)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));
            if (sqldb == null || sqldb.Connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.Connection)
            {
                var query = new StringBuilder();
                query.Append(@$"DELETE FROM [HealthMedAgendamento].[dbo].[Agendamento] WHERE [Id] = '{idAgendamento.ToString()}' ");
                sqldb.Connection.Query<Paciente>(query.ToString(), param: null);
                sqldb.Connection.Close();
            }
        }

        private bool VerificarDisponibilidadeMedico(Agendamento agendamento)
        {
            DisponibilidadeMedicoRepository disponibilidadeMedicoRepository = new(this._config);
            IEnumerable<DisponibilidadeMedico> horariosDisponiveisMedico = [];
            disponibilidadeMedicoRepository.Get(agendamento.IdMedico.ToString());

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
            IEnumerable<Agendamento> agendamentos = this.Get(agendamento.IdMedico.ToString(), agendamento.DataInicio);

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
        /// <param name="idAgendamento"></param>
        public async void NotificarAgendamento(Guid idAgendamento)
        {
            AgendamentoRepository agendamentoRepository = new(this._config);
            Agendamento? agendamento = agendamentoRepository.GetAgendamento(idAgendamento.ToString());

            PacienteRepository pacienteRepository = new(this._config);
            Paciente? paciente = pacienteRepository.Get(agendamento.IdPaciente.ToString());

            MedicoRepository medicoRepository = new(this._config);
            Medico? medico = medicoRepository.Get(agendamento.IdMedico.ToString());

            if (paciente!= null && medico != null)
            {
                MailRequest mail = new()
                {
                    To = medico.Email,
                    Subject = "Health&Med - Solicitação de consulta",
                    Body = @$"Olá, Dr. <b>{medico.Nome}</b>! 
                    O paciente <b>{paciente.Nome}</b> solicitou uma consulta.
                    Data e horário: {agendamento.DataInicio:dd/MM/yyyy} às {agendamento.DataInicio:HH:mm}"
                };
                MailService mailService = new(this._config);
                await mailService.SendEmailAsync(mail);
            }
            
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
        /// <param name="idAgendamento"></param>
        public async void NotificarAprovacao(Guid idAgendamento)
        {
            AgendamentoRepository agendamentoRepository = new(this._config);
            Agendamento? agendamento = agendamentoRepository.GetAgendamento(idAgendamento.ToString());
            
            PacienteRepository pacienteRepository = new(this._config);
            Paciente? paciente = pacienteRepository.Get(agendamento.IdPaciente.ToString());

            MedicoRepository medicoRepository = new(this._config);
            Medico? medico = medicoRepository.Get(agendamento.IdMedico.ToString());

            if (paciente != null && medico != null)
            {
                MailRequest mail = new()
                {
                    To = paciente.Email,
                    Subject = "Health&Med - Solicitação de Consulta Aprovada",
                    Body = @$"Olá, <b>{paciente.Nome}</b>.
                    Sua solicitação de consulta foi aprovada pelo médico <b>{medico.Nome}</b>.
                    Data e horário: {agendamento.DataInicio:dd/MM/yyyy} às {agendamento.DataInicio:HH:mm}"
                };
                MailService mailService = new(this._config);
                await mailService.SendEmailAsync(mail);
            }
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
        /// <param name="idAgendamento"></param>
        public async void NotificarRecusa(Guid idAgendamento)
        {
            AgendamentoRepository agendamentoRepository = new(this._config);
            Agendamento? agendamento = agendamentoRepository.GetAgendamento(idAgendamento.ToString());
            
            PacienteRepository pacienteRepository = new(this._config);
            Paciente? paciente = pacienteRepository.Get(agendamento.IdPaciente.ToString());

            MedicoRepository medicoRepository = new(this._config);
            Medico? medico = medicoRepository.Get(agendamento.IdMedico.ToString());

            if (paciente != null && medico != null)
            {
                MailRequest mail = new()
                {
                    To = paciente.Email,
                    Subject = "Health&Med - Solicitação de Consulta Recusada",
                    Body = @$"Olá, <b>{paciente.Nome}</b>
                    Sua solicitação de consulta foi recusada pelo médico <b>{medico.Nome}</b>.
                    Data e horário: {agendamento.DataInicio:dd/MM/yyyy} às {agendamento.DataInicio:HH:mm}"
                };
                MailService mailService = new(this._config);
                await mailService.SendEmailAsync(mail);
            }
        }

        public async void NotificarCancelamento(Guid idAgendamento, String motivo)
        {
            AgendamentoRepository agendamentoRepository = new(this._config);
            Agendamento? agendamento = agendamentoRepository.GetAgendamento(idAgendamento.ToString());

            PacienteRepository pacienteRepository = new(this._config);
            Paciente? paciente = pacienteRepository.Get(agendamento.IdPaciente.ToString());

            MedicoRepository medicoRepository = new(this._config);
            Medico? medico = medicoRepository.Get(agendamento.IdMedico.ToString());

            if (paciente != null && medico != null)
            {
                MailRequest mail = new()
                {
                    To = medico.Senha,
                    Subject = "Health&Med - Consulta Cancelada",
                    Body = @$"Olá, Dr. <b>{medico.Nome}</b>!
                    A consulta marcada para o dia
                    {agendamento.DataInicio:dd/MM/yyyy} às {agendamento.DataInicio:HH:mm}
                    foi cancelada pelo paciente <b>{paciente.Nome}</b>.
                    Motivo: {motivo}"
                };

                MailService mailService = new(this._config);
                await mailService.SendEmailAsync(mail);
            }
        }
    }
}
