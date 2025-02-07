using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using System.Reflection.Metadata.Ecma335;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Teste Deploy API");
        }
    }

    [Route("auth/[action]")]
    [ApiController]
    public class AuthController(ILogger<PacienteController> logger,
        PacienteRepository pacienteRepository,
        MedicoRepository medicoRepository) : ControllerBase
    {

        private readonly ILogger<PacienteController> _logger = logger;
        private readonly PacienteRepository _pacienteRepository = pacienteRepository;
        private readonly MedicoRepository _medicoRepository = medicoRepository;

        [HttpPost("LoginMedico")]
        public IActionResult LoginMedico(string crm, string password)
        {
            string? token = _medicoRepository.GetToken(crm, password, true); // true = Médico

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Médico logado com sucesso.");
                return Ok(new { token });
            }
            else
            {
                _logger.LogError("Usuário ou senha inválida.");
                return Unauthorized(new { message = "Usuário ou senha inválida." });
            }
        }

        
        [HttpPost("LoginPaciente")]
        public IActionResult LoginPaciente(string email, string password)
        {
            string? token = _pacienteRepository.GetToken(email, password, false); // false = Paciente

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Paciente logado com sucesso.");
                return Ok(new { token });
            }
            else
            {
                _logger.LogError("Usuário ou senha inválida.");
                return Unauthorized(new { message = "Usuário ou senha inválida." });
            }

        }

    }
}
