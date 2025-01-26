using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("api/Medico/[controller]")]
    [ApiController]
    public class DisponibilidadeMedicoController(ILogger<DisponibilidadeMedicoController> logger, DisponibilidadeMedicoRepository disponibilidadeMedicoRepository) : ControllerBase
    {
        private readonly ILogger<DisponibilidadeMedicoController> _logger = logger;
        private readonly DisponibilidadeMedicoRepository _disponibilidadeMedicoRepository = disponibilidadeMedicoRepository;

        // POST: api/<MedicoController>
        /// <summary>
        /// Cadastro de Médico
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] DisponibilidadeMedico value)
        {
            Guid idDisponibilidadeMedico = _disponibilidadeMedicoRepository.Post(value);
            _logger.LogInformation("Período de Disponibilidade cadastrado com sucesso.");
            return Ok(new
            {
                Message = "Período de Disponibilidade cadastrado com sucesso.",
                Id = idDisponibilidadeMedico
            });
        }
    }
}
