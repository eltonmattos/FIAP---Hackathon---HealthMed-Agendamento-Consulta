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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(ILogger<PacienteController> logger,
        PacienteRepository pacienteRepository,
        MedicoRepository medicoRepository) : ControllerBase
    {

        private readonly ILogger<PacienteController> _logger = logger;
        private readonly PacienteRepository _pacienteRepository = pacienteRepository;
        private readonly MedicoRepository _medicoRepository = medicoRepository;

        [HttpGet]
        [Authorize]
        public IActionResult PrivateAPI()
        {
            var list = new[]
            {
                new { Code = 1, Name = "This end point is restricted " },
                new { Code = 2, Name = "You need to login to see this" }
            }.ToList();

            return Ok(list);
        }

        [HttpGet]
        public IActionResult PublicAPI()
        {
            var list = new[]
            {
                new { Code = 1, Name = "This end point can be accessed by Public" },
                new { Code = 2, Name = "Whatever" }
            }.ToList();

            return Ok(list);
        }

        [HttpPost("LoginMedico")]
        public IActionResult LoginMedico(string email, string password)
        {
            string? token = _medicoRepository.GetToken(email, password, true); // true = Médico

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Médico logado com sucesso.");
                return Ok(new { token });
            }

            _logger.LogError("Usuário ou senha inválida.");
            return Unauthorized(new { message = "Usuário ou senha inválida." });
        }


        [HttpPost("LoginPaciente")]
        public IActionResult LoginPaciente(string email, string password)
        {
            string? token = _medicoRepository.GetToken(email, password, false); // false = Paciente

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Paciente logado com sucesso.");
                return Ok(new { token });
            }

            _logger.LogError("Usuário ou senha inválida.");
            return Unauthorized(new { message = "Usuário ou senha inválida." });
        }

    }
}
