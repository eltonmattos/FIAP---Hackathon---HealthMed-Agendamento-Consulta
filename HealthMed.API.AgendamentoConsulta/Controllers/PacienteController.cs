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
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: PacienteController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: PacienteController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: api/<PacienteController>
        /// <summary>
        /// Cadastro de Paciente
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Paciente value)
        {
            Guid idPaciente = _pacienteRepository.Post(value);
            _logger.LogInformation("Paciente cadastrado com sucesso.");
            return Ok(new
            {
                Message = "Paciente cadastrado com sucesso.",
                Id = idPaciente
            });
        }

        //// POST: PacienteController/Create
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
        //        return View();
        //    }
        //}

        //// GET: PacienteController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: PacienteController/Edit/5
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
        //        return View();
        //    }
        //}

        //// GET: PacienteController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: PacienteController/Delete/5
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
        //        return View();
        //    }
        //}
    }
}
