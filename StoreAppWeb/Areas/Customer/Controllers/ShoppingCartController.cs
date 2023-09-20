using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;

namespace StoreAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
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
                    includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var scart in ShoppingCartVM.ShoppingCartList)
            {
                scart.Price = GetPriceByQuantity(scart);
                ShoppingCartVM.OrderHeader.OrderTotal += (scart.Price * scart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult OrderSummary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(s => s.AppUserId == userId,
                    includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUserRepo.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.AppUser.StreetAddres;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.AppUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode;

            foreach (var scart in ShoppingCartVM.ShoppingCartList)
            {
                scart.Price = GetPriceByQuantity(scart);
                ShoppingCartVM.OrderHeader.OrderTotal += (scart.Price * scart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("OrderSummary")]
		public IActionResult OrderSummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(s => s.AppUserId == userId,
                    includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;

			ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUserRepo.Get(u => u.Id == userId);

			foreach (var scart in ShoppingCartVM.ShoppingCartList)
			{
				scart.Price = GetPriceByQuantity(scart);
				ShoppingCartVM.OrderHeader.OrderTotal += (scart.Price * scart.Count);
			}

            if (ShoppingCartVM.OrderHeader.AppUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayed;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

            _unitOfWork.OrderHeaderRepo.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _unitOfWork.OrderDetailRepo.Add(orderDetail);
                _unitOfWork.Save();
            }

			return View(ShoppingCartVM);
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
