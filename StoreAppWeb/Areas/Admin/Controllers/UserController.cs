﻿using DataAccess.IRepositories;
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
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            string roleId = _db.UserRoles.FirstOrDefault(r => r.UserId == userId).RoleId;

            RoleManagmentVM roleVM = new RoleManagmentVM()
            {
                AppUser = _db.AppUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),

                RoleList = _db.Roles.Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Name
                }),

                CompanyList = _db.Companies.Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            roleVM.AppUser.Role = _db.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string roleId = _db.UserRoles.FirstOrDefault(r => r.UserId == roleManagmentVM.AppUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            if (!(roleManagmentVM.AppUser.Role == oldRole))
            {
                AppUser appUser = _db.AppUsers.FirstOrDefault(u => u.Id == roleManagmentVM.AppUser.Id);

                if (roleManagmentVM.AppUser.Role == StaticDetails.Role_Company)
                {
                    appUser.CompanyId = roleManagmentVM.AppUser.CompanyId;
                }

                if (oldRole == StaticDetails.Role_Company)
                {
                    appUser.CompanyId = null;
                }

                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(appUser, roleManagmentVM.AppUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<AppUser> usersList = _db.AppUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in usersList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

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
            var userFromDb = _db.AppUsers.FirstOrDefault(u => u.Id == id);

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

            _db.SaveChanges();

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}