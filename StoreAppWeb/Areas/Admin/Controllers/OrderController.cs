using DataAccess.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace StoreAppWeb.Areas.Admin.Controllers
{
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
		{
			return View();
		}

		#region ApiCalls

		[HttpGet]
		public IActionResult GetAll()
		{
			List<OrderHeader> orderHeaders = _unitOfWork.OrderHeaderRepo.GetAll(includeProperties: "AppUser").ToList();

			return Json(new { data = orderHeaders });
		}

		#endregion
	}
}
