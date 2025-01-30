using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
