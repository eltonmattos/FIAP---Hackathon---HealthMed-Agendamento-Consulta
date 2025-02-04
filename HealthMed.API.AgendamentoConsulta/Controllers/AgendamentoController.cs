using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("api/Paciente/[controller]")]
    [ApiController]
    public class AgendamentoController(ILogger<AgendamentoController> logger, AgendamentoRepository agendamentoRepository) : ControllerBase
    {

        private readonly ILogger<AgendamentoController> _logger = logger;
        private readonly AgendamentoRepository _agendamentoRepository = agendamentoRepository;

        // POST: api/<AgendamentoController>
        /// <summary>
        /// Cadastro de Agendamento
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize(Roles = "Paciente")]

        [HttpPost("/api/Agendamento/")]
        public IActionResult Post([FromBody] Agendamento value)
        {
            Guid idAgendamento = _agendamentoRepository.Post(value);
            _logger.LogInformation("Agendamento cadastrado com sucesso.");

#if (!DEBUG)
            _agendamentoRepository.NotificarAgendamento(idAgendamento);
#endif

            return Ok(new
            {
                Message = "Agendamento cadastrado com sucesso.",
                Id = idAgendamento
            });
        }

        // GET: AgendamentoController
        /// <summary>
        /// Obter Agendamentos
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("/api/Agendamento/{idMedico}")]
        [Authorize(Roles = "Medico")]
        public IActionResult Get(String idMedico)
        {
            IEnumerable<Agendamento> agendamentos = _agendamentoRepository.Get(idMedico);
            return Ok(agendamentos);
        }

        // GET: AgendamentoController
        /// <summary>
        /// Obter Agendamentos por Data
        /// </summary>
        /// <param name="idMedico"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpGet("/api/Agendamento/{idMedico}/{Data}")]
        public IActionResult Get(String idMedico, String Data)
        {
            DateTime dateFormat = DateTime.Parse(Data);
            IEnumerable<Agendamento> agendamentos = _agendamentoRepository.Get(idMedico, dateFormat);
            return Ok(agendamentos);
        }


        // PUT: api/<AgendamentoController>
        /// <summary>
        /// Aprovar Agendamento
        /// </summary>
        /// <param name="idAgendamento"></param>
        /// <returns></returns>
        [Authorize(Roles = "Medico")]

        [HttpPut("/api/Agendamento/AprovarAgendamento/{idAgendamento}")]
        public IActionResult AprovarAgendamento(String idAgendamento)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.Aprovado);
#if (!DEBUG)
            _agendamentoRepository.NotificarAprovacao(new Guid(idAgendamento));
#endif
            _logger.LogInformation("Agendamento aprovado com sucesso.");

            return Ok(new
            {
                Message = "Agendamento aprovado com sucesso.",
                Id = idAgendamento
            });
        }

        // PUT: api/<AgendamentoController>
        /// <summary>
        /// Recusar Agendamento
        /// </summary>
        /// <param name="idAgendamento"></param>
        /// <returns></returns>
        [Authorize(Roles = "Medico")]

        [HttpPut("/api/Agendamento/RecusarAgendamento/{idAgendamento}")]
        public IActionResult RecusarAgendamento(String idAgendamento)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.RecusadoPeloMedico);

#if (!DEBUG)
            _agendamentoRepository.NotificarRecusa(new Guid(idAgendamento));
#endif
            _logger.LogInformation("Agendamento recusado.");

            return Ok(new
            {
                Message = "Agendamento recusado.",
                Id = idAgendamento
            });
        }

        // PUT: api/<AgendamentoController>
        /// <summary>
        /// Cancelar Agendamento
        /// </summary>
        /// <param name="idAgendamento"></param>
        /// <param name="Motivo"></param>
        /// <returns></returns>
        [Authorize(Roles = "Paciente")]

        [HttpPut("/api/Agendamento/CancelarAgendamento/{idAgendamento}")]
        public IActionResult CancelarAgendamento(String idAgendamento, [BindRequired] String Motivo)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.RecusadoPeloMedico, Motivo);

#if (!DEBUG)
            _agendamentoRepository.NotificarCancelamento(new Guid(idAgendamento), Motivo);
#endif
            _logger.LogInformation("Agendamento cancelado.");

            return Ok(new
            {
                Message = "Agendamento recusado.",
                Id = idAgendamento
            });
        }
    }
}
