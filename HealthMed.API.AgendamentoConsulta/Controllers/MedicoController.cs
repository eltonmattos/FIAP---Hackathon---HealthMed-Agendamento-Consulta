using HealthMed.API.AgendamentoConsulta.Models;
using HealthMed.API.AgendamentoConsulta.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly ILogger<MedicoController> _logger;
        private readonly MedicoRepository _medicoRepository;

        public MedicoController(ILogger<MedicoController> logger, MedicoRepository medicoRepository)
        {
            _logger = logger;
            _medicoRepository = medicoRepository;
        }

        //// GET: MedicoController
        //public ActionResult Index()
        //{
        //    //return View();
        //}

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
        [HttpPost]
        public IActionResult Post([FromBody] Medico value)
        {
            Guid idMedico = _medicoRepository.Post(value);
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
