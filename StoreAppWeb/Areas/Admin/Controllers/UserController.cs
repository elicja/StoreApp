using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using NuGet.Common;
using StoreApp.DataAccess.Data;
using StoreApp.Models;
using Utility;

namespace StoreAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<AppUser> usersList = _db.AppUsers.Include(u => u.company).ToList();

            return Json(new { data = usersList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
