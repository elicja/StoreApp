using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreAppWebRazor_Temp.Data;
using StoreAppWebRazor_Temp.Models;

namespace StoreAppWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;
        public Category Category { get; set; }

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet(int? categoryId)
        {
            if (categoryId != null && categoryId != 0)
            {
                Category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();

                TempData["success"] = "Category updated successfully";

                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
