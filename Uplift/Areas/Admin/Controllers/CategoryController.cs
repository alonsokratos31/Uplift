using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository;
using Uplift.Models;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        
        private readonly IUniOfWork _uniOfWork;

        public CategoryController(IUniOfWork uniOfWork)
        {
            _uniOfWork = uniOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = _uniOfWork.Category.Get(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _uniOfWork.Category.Add(category);
                }
                else
                {
                    _uniOfWork.Category.Update(category);
                }
                _uniOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(category);

        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _uniOfWork.Category.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromdb = _uniOfWork.Category.Get(id);
            if (objFromdb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            _uniOfWork.Category.Remove(objFromdb);
            _uniOfWork.Save();

            return Json(new { success = true, message = "Delete successful." });
        }

        #endregion
    }
}
