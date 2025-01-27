using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController(ILogger<MedicoController> logger, MedicoRepository medicoRepository) : ControllerBase
    {
        private readonly ILogger<MedicoController> _logger = logger;
        private readonly MedicoRepository _medicoRepository = medicoRepository;

        //// GET: MedicoController
        /// <summary>
        /// Obter Medicos
        /// 
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("/api/Medico/")]
        public IActionResult Get()
        {
            IEnumerable<object> medicos = _medicoRepository.Get();
            return Ok(medicos);
        }

        //// GET: MedicoController/Details/5
        //public ActionResult Details(int id)
        //{
        //    //return View();
        //}

        //// GET: MedicoController/Create
        //public ActionResult Create()
        //{
        //    //return View();
        //}

        // POST: api/<MedicoController>
        /// <summary>
        /// Cadastro de Médico
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("/api/Medico/")]
        public IActionResult Post([FromBody] Medico value)
        {
            Guid idMedico = _medicoRepository.Post(value);
            _logger.LogInformation("Médico cadastrado com sucesso: {IdMedico}", idMedico);
            return Ok(new
            {
                Message = "Médico cadastrado com sucesso.",
                Id = idMedico
            });
        }

        //// POST: MedicoController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        //return View();
        //    }
        //}

        //// GET: MedicoController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    //return View();
        //}

        //// POST: MedicoController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        //return View();
        //    }
        //}

        //// GET: MedicoController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    //return View();
        //}

        //// POST: MedicoController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        //return View();
        //    }
        //}
    }
}
