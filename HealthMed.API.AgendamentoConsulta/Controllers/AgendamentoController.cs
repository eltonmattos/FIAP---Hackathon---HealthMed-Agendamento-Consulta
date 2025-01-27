using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
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
        //[Authorize]
        [HttpPost]
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
        /// <param name="value"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet]
        //public IActionResult Get([FromBody] DateTime value)
        //{
        //    IEnumerable<Agendamento> agendamentos = _agendamentoRepository.Get(value);
        //    return Ok(agendamentos);
        //}

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    IEnumerable<Agendamento> agendamentos = _agendamentoRepository.Get();
        //    return Ok(agendamentos);
        //}

        //    // GET: AgendamentoController/Details/5
        //    public ActionResult Details(int id)
        //    {
        //        return View();
        //    }

        //    // GET: AgendamentoController/Create
        //    public ActionResult Create()
        //    {
        //        return View();
        //    }

        //    // POST: AgendamentoController/Create
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Create(IFormCollection collection)
        //    {
        //        try
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }

        //    // GET: AgendamentoController/Edit/5
        //    public ActionResult Edit(int id)
        //    {
        //        return View();
        //    }

        //    // POST: AgendamentoController/Edit/5
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit(int id, IFormCollection collection)
        //    {
        //        try
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }

        //    // GET: AgendamentoController/Delete/5
        //    public ActionResult Delete(int id)
        //    {
        //        return View();
        //    }

        //    // POST: AgendamentoController/Delete/5
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Delete(int id, IFormCollection collection)
        //    {
        //        try
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }
    }
}
