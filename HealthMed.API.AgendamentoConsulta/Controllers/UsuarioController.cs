using Microsoft.AspNetCore.Mvc;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
