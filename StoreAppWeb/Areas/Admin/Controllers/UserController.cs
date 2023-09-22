using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            RoleManagmentVM roleVM = new RoleManagmentVM()
            {
                AppUser = _unitOfWork.AppUserRepo.Get(u => u.Id == userId, includeProperties: "Company"),

                RoleList = _roleManager.Roles.Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Name
                }),

                CompanyList = _unitOfWork.CompanyRepo.GetAll().Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            roleVM.AppUser.Role = _userManager.GetRolesAsync(_unitOfWork.AppUserRepo.Get(u => u.Id == userId))
                                  .GetAwaiter().GetResult().FirstOrDefault();

            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.AppUserRepo.Get(u => u.Id == roleManagmentVM.AppUser.Id))
                                  .GetAwaiter().GetResult().FirstOrDefault();

            AppUser appUser = _unitOfWork.AppUserRepo.Get(u => u.Id == roleManagmentVM.AppUser.Id);

            if (!(roleManagmentVM.AppUser.Role == oldRole))
            {
                if (roleManagmentVM.AppUser.Role == StaticDetails.Role_Company)
                {
                    appUser.CompanyId = roleManagmentVM.AppUser.CompanyId;
                }

                if (oldRole == StaticDetails.Role_Company)
                {
                    appUser.CompanyId = null;
                }

                _unitOfWork.AppUserRepo.Update(appUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(appUser, roleManagmentVM.AppUser.Role).GetAwaiter().GetResult();
            }
            else if (oldRole == StaticDetails.Role_Company && appUser.CompanyId != roleManagmentVM.AppUser.CompanyId)
            {
                appUser.CompanyId = roleManagmentVM.AppUser.CompanyId;

                _unitOfWork.AppUserRepo.Update(appUser);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<AppUser> usersList = _unitOfWork.AppUserRepo.GetAll(includeProperties: "Company").ToList();

            foreach (var user in usersList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new() 
                    { 
                        Name = ""
                    };
                }
            }

            return Json(new { data = usersList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var userFromDb = _unitOfWork.AppUserRepo.Get(u => u.Id == id);

            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _unitOfWork.AppUserRepo.Update(userFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
