using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository;
using Uplift.Models.ViewModels;
using Uplift.Utility;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUniOfWork _uniOfWork;

        public OrderController(IUniOfWork uniOfWork)
        {
            _uniOfWork = uniOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderViewModel orderVM = new OrderViewModel()
            {
                OrderHeader = _uniOfWork.OrderHeader.Get(id),
                OrderDetails = _uniOfWork.OrderDetails.GetAll(filter: o => o.OrderHeaderId == id)
            };

            return View(orderVM);
        }

        public IActionResult Approve(int id)
        {
            var orderFromDb = _uniOfWork.OrderHeader.Get(id);
            if (orderFromDb == null)
            {
                return NotFound();
            }
            _uniOfWork.OrderHeader.ChangeOrderStatus(id, SD.StatusApproved);
            return View(nameof(Index));
        }

        public IActionResult Reject(int id)
        {
            var orderFromDb = _uniOfWork.OrderHeader.Get(id);
            if (orderFromDb == null)
            {
                return NotFound();
            }
            _uniOfWork.OrderHeader.ChangeOrderStatus(id, SD.StatusRejected);
            return View(nameof(Index));
        }

        #region API CALLS

        public IActionResult GetAllOrders()
        {

            return Json(new { data = _uniOfWork.OrderHeader.GetAll() });
        }

        public IActionResult GetAllPendingOrders()
        {

            return Json(new { data = _uniOfWork.OrderHeader.GetAll(filter: o => o.Status == SD.StatusSubmitted) });
        }

        public IActionResult GetAllApprovedOrdes()
        {

            return Json(new { data = _uniOfWork.OrderHeader.GetAll(filter: o => o.Status == SD.StatusApproved) });
        }


        #endregion
    }
}
