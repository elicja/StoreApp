using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreAppWebRazor_Temp.Data;
using StoreAppWebRazor_Temp.Models;

namespace StoreAppWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;
        public Category Category { get; set; }

        public DeleteModel(AppDbContext db)
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
            Category categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == Category.Id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();

            TempData["success"] = "Category deleted successfully";

            return RedirectToPage("Index");
        }
    }
}
