using Microsoft.AspNetCore.Mvc;
using StoreAppWeb.Data;
using StoreAppWeb.Models;

namespace StoreAppWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Category> categoryList = _db.Categories.ToList();
            return View(categoryList);
        }
    }
}
