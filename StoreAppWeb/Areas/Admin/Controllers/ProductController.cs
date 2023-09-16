using DataAccess.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using NuGet.Common;
using StoreApp.Models;

namespace StoreAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.ProductRepo.GetAll().ToList();

            return View(productList);
        }

        public IActionResult Create()
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product()
            };

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Add(productVM.Product);
                _unitOfWork.Save();

                TempData["success"] = "Product created successfully";

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                return View(productVM);
            }
        }

        public IActionResult Edit(int? productId)
        {
            if (productId == null || productId == default)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.ProductRepo.Get(p => p.Id == productId);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Update(product);
                _unitOfWork.Save();

                TempData["success"] = "Product updated successfully";

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? productId)
        {
            if (productId == null || productId == default)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.ProductRepo.Get(p => p.Id == productId);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? productId)
        {
            Product productFromDb = _unitOfWork.ProductRepo.Get(p => p.Id == productId);

            if (productFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepo.Delete(productFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");
        }
    }
}
