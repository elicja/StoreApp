using DataAccess.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utility;

namespace StoreAppWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(StaticDetails.SessionShoppingCart) == null)
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart,
                    _unitOfWork.ShoppingCartRepo.GetAll(s => s.AppUserId == claim.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(StaticDetails.SessionShoppingCart));
            }
            else
            {
                HttpContext.Session.Clear();

                return View(0);
            }
        }
    }
}
