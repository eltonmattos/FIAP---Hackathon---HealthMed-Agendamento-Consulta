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
        /// <param name="notify"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize(Roles = "Paciente")]

        [HttpPost("/api/Agendamento/")]
        public IActionResult Post([FromQuery] Boolean notify, [FromBody] Agendamento value)
        {
            Guid idAgendamento = _agendamentoRepository.Post(value);

            if (notify)
                _agendamentoRepository.NotificarAgendamento(idAgendamento);

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
        /// <param name="notify"></param>
        /// <param name="idAgendamento"></param>
        /// <returns></returns>
        [Authorize(Roles = "Medico")]

        [HttpPut("/api/Agendamento/AprovarAgendamento/{idAgendamento}")]
        public IActionResult AprovarAgendamento([FromQuery] Boolean notify, String idAgendamento)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.Aprovado);

            if (notify)
                _agendamentoRepository.NotificarAprovacao(new Guid(idAgendamento));

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
        /// <param name="notify"></param>
        /// <param name="idAgendamento"></param>
        /// <returns></returns>
        [Authorize(Roles = "Medico")]

        [HttpPut("/api/Agendamento/RecusarAgendamento/{idAgendamento}")]
        public IActionResult RecusarAgendamento([FromQuery] Boolean notify, String idAgendamento)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.RecusadoPeloMedico);

            if (notify)
                _agendamentoRepository.NotificarRecusa(new Guid(idAgendamento));

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
        /// <param name="notify"></param>
        /// <param name="idAgendamento"></param>
        /// <param name="Motivo"></param>
        /// <returns></returns>
        [Authorize(Roles = "Paciente")]

        [HttpPut("/api/Agendamento/CancelarAgendamento/{idAgendamento}")]
        public IActionResult CancelarAgendamento([FromQuery] Boolean notify, String idAgendamento, [FromBody][BindRequired] String Motivo)
        {
            _agendamentoRepository.AlterarStatusAgendamento(new Guid(idAgendamento), (int)StatusAgendamento.RecusadoPeloMedico, Motivo);

            if (notify)
                _agendamentoRepository.NotificarCancelamento(new Guid(idAgendamento), Motivo);

            _logger.LogInformation("Agendamento cancelado.");

            return Ok(new
            {
                Message = "Agendamento cancelado.",
                Id = idAgendamento
            });
        }
    }
}
