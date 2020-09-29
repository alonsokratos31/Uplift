using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository;
using Uplift.Models;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FrequencyController : Controller
    {
        private readonly IUniOfWork _uniOfWork;

        public FrequencyController(IUniOfWork uniOfWork)
        {
            _uniOfWork = uniOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Frequency frequency = new Frequency();
            if (id == null)
            {
                return View(frequency);
            }
            frequency = _uniOfWork.Frequency.Get(id.GetValueOrDefault());

            if (frequency == null)
            {
                return NotFound();
            }

            return View(frequency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Frequency frequency)
        {
            if (ModelState.IsValid)
            {
                if (frequency.Id == 0)
                {
                    _uniOfWork.Frequency.Add(frequency);
                }
                else
                {
                    _uniOfWork.Frequency.Update(frequency);
                }
                _uniOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(frequency);

        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _uniOfWork.Frequency.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromdb = _uniOfWork.Frequency.Get(id);
            if (objFromdb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            _uniOfWork.Frequency.Remove(objFromdb);
            _uniOfWork.Save();

            return Json(new { success = true, message = "Delete successful." });
        }

        #endregion
    }
}
