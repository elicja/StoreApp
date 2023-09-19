using Microsoft.AspNetCore.Mvc;

namespace StoreAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
