using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Models;
using Models.ViewModels;
using System.Security.Claims;

namespace StoreAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(s => s.AppUserId == userId,
                    includeProperties: "Product")
            };

            foreach (var scart in ShoppingCartVM.ShoppingCartList)
            {
                scart.Price = GetPriceByQuantity(scart);
                ShoppingCartVM.OrderTotal += (scart.Price * scart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult OrderSummary()
        {
            return View();
        }

        public IActionResult Plus(int shoppingCartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCartRepo.Get(s => s.Id == shoppingCartId);

            shoppingCartFromDb.Count += 1;

            _unitOfWork.ShoppingCartRepo.Update(shoppingCartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int shoppingCartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCartRepo.Get(s => s.Id == shoppingCartId);

            if (shoppingCartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepo.Delete(shoppingCartFromDb);
            }
            else
            {
                shoppingCartFromDb.Count -= 1;

                _unitOfWork.ShoppingCartRepo.Update(shoppingCartFromDb);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int shoppingCartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCartRepo.Get(s => s.Id == shoppingCartId);

            _unitOfWork.ShoppingCartRepo.Delete(shoppingCartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceByQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 1000)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
