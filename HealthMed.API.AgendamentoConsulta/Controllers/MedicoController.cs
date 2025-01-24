using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.API.AgendamentoConsulta.Controllers
{
    public class MedicoController : Controller
    {
        // GET: MedicoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MedicoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MedicoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedicoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MedicoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MedicoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MedicoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MedicoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
