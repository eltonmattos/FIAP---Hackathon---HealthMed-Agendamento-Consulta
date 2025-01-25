using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
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
    }
}
