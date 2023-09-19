using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using NuGet.Common;
using StoreApp.Models;
using Utility;

namespace StoreAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties:"Category").ToList();

            return View(productList);
        }

        public IActionResult Upsert(int? id)
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

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.ProductRepo.Get(p => p.Id == id);

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"imgs\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImgUrl))
                    {
                        var oldImgPath = 
                            Path.Combine(wwwRootPath, productVM.Product.ImgUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImgUrl = @"\imgs\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepo.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepo.Update(productVM.Product);
                }
                
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

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category").ToList();

            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.ProductRepo.Get(p => p.Id == id);

            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, 
                                          productToDelete.ImgUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }

            _unitOfWork.ProductRepo.Delete(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
