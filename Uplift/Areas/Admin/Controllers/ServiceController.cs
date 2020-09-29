using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository;
using Uplift.Models;
using Uplift.Models.ViewModels;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ServiceController : Controller
    {

        private readonly IUniOfWork _uniOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public ServiceVM ServVM { get; set; }

        public ServiceController(IUniOfWork uniOfWork, IWebHostEnvironment hostEnvironment)
        {
            _uniOfWork = uniOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ServVM = new ServiceVM()
            {
                service = new Service(),
                CategoryList = _uniOfWork.Category.GetCategoryListForDropDown(),
                FrequencyList = _uniOfWork.Frequency.GetFrequencyListForDropDown(),
                
            };

            if (id != null)
            {
                ServVM.service = _uniOfWork.Service.Get(id.GetValueOrDefault());
            }
            
            return View(ServVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (ServVM.service.Id == 0)
                {
                    //new service
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\services");
                    var extension = Path.GetExtension(files[0].FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    ServVM.service.ImageUrl = @"\images\services\" + fileName + extension;
                    _uniOfWork.Service.Add(ServVM.service);
                }
                else
                {
                    //Edit service
                    var serviceFromDb = _uniOfWork.Service.Get(ServVM.service.Id);
                    if (files.Count > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, @"images\services");
                        var extension_new = Path.GetExtension(files[0].FileName);

                        var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension_new), FileMode.Create))
                        {
                            files[0].CopyTo(fileStreams);
                        }

                        ServVM.service.ImageUrl = @"\images\services\" + fileName + extension_new;
                    }
                    else
                    {
                        ServVM.service.ImageUrl = serviceFromDb.ImageUrl;
                    }

                    _uniOfWork.Service.Update(ServVM.service);
                }

                _uniOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ServVM.CategoryList = _uniOfWork.Category.GetCategoryListForDropDown();
                ServVM.FrequencyList = _uniOfWork.Frequency.GetFrequencyListForDropDown();
                return View(ServVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _uniOfWork.Service.GetAll(includeProperties: "Category,Frequency") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var serviceFromdb = _uniOfWork.Service.Get(id);
            string webRootPath = _hostEnvironment.WebRootPath;

            var imagePath = Path.Combine(webRootPath, serviceFromdb.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (serviceFromdb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            _uniOfWork.Service.Remove(serviceFromdb);
            _uniOfWork.Save();

            return Json(new { success = true, message = "Delete successful." });
        }

        #endregion
    }
}
