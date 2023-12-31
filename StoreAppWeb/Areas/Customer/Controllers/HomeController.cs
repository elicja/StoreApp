using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using StoreApp.Models;
using System.Diagnostics;
using System.Security.Claims;
using Utility;

namespace StoreAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category,ProductImgs");

            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.ProductRepo.Get(p => p.Id == productId, includeProperties: "Category,ProductImgs"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.AppUserId = userId;

            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCartRepo.Get(s => s.AppUserId == userId
                && s.ProductId == shoppingCart.ProductId);

            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;

                _unitOfWork.ShoppingCartRepo.Update(shoppingCartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCartRepo.Add(shoppingCart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart,
                    _unitOfWork.ShoppingCartRepo.GetAll(s => s.AppUserId == userId).Count());
            }

            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
