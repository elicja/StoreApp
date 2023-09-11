using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreAppWebRazor_Temp.Data;
using StoreAppWebRazor_Temp.Models;

namespace StoreAppWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        public Category Category { get; set; }

        public CreateModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(Category);
            _db.SaveChanges();

            TempData["success"] = "Category created successfully";

            return RedirectToPage("Index");
        }
    }
}
