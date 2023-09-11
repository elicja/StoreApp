using Microsoft.AspNetCore.Mvc;
using StoreApp.DataAccess.Data;
using StoreApp.Models;

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name of the category and isplay order cannot be the same.");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();

                TempData["success"] = "Category created successfully";

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? categoryId)
        {
            if (categoryId == null || categoryId == default)
            {
                return NotFound();
            }
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();

                TempData["success"] = "Category updated successfully";

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? categoryId)
        {
            if (categoryId == null || categoryId == default)
            {
                return NotFound();
            }
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? categoryId)
        {
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();

            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
