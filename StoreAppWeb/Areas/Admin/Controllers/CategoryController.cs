using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.DataAccess.Data;
using StoreApp.Models;
using Utility;

namespace StoreAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork.CategoryRepo.GetAll().ToList();
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
                _unitOfWork.CategoryRepo.Add(category);
                _unitOfWork.Save();

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
            Category categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);

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
                _unitOfWork.CategoryRepo.Update(category);
                _unitOfWork.Save();

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
            Category categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? categoryId)
        {
            Category categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepo.Delete(categoryFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Category deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
