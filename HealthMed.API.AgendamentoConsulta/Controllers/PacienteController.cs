using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController(ILogger<PacienteController> logger, PacienteRepository pacienteRepository) : ControllerBase
    {
        private readonly ILogger<PacienteController> _logger = logger;
        private readonly PacienteRepository _pacienteRepository = pacienteRepository;

        //// GET: PacienteController
        /// <summary>
        /// Obter Paciente por Id
        /// 
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("/api/Paciente/{idPaciente}")]
        public IActionResult Get(String idPaciente)
        {
            Paciente? paciente = _pacienteRepository.Get(idPaciente);
            return Ok(paciente);
        }

        // POST: api/<PacienteController>
        /// <summary>
        /// Cadastro de Paciente
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("/api/Paciente/")]
        public IActionResult Post([FromBody] Paciente value)
        {
            Guid idPaciente = _pacienteRepository.Post(value);
            _logger.LogInformation("Paciente cadastrado com sucesso.");
            try
            {
                return Ok(new
                {
                    Message = "Paciente cadastrado com sucesso.",
                    Id = idPaciente
                });
            }
            catch (FormatException ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
