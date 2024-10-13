﻿using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly AccountsService _accountsService;

        public UserController(AccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        /// <summary>
        /// Open Admin Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        public IActionResult Index()
        {
            var users = _accountsService.GetAll();
            return View(users);
        }

        /// <summary>
        /// Open Admin User Details Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        /// <param name="id">Target user Id</param>
        public IActionResult Details(Guid id)
        {
            var user = _accountsService.Get(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Change password for user
        /// </summary>
        /// <returns>User Details Page</returns>
        /// <param name="changePassword">Target changePassword model</param>  
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ChangePasswordForm", changePassword);
            }

            _accountsService.ChangePassword(changePassword);

            var redirectUrl = Url.Action("Details", new { id = changePassword.UserId });

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Open Edit User Page
        /// </summary>
        /// <returns>Admin Edit User View</returns>
        /// <param name="editUser">Target EditUser model</param> 
        public IActionResult Edit(EditUser editUser) => View(editUser);

        /// <summary>
        /// Update target user
        /// </summary>
        /// <returns>User Details Page if success; otherwise Edit User View</returns>
        /// <param name="editUser">Target EditUser model</param>  
        [HttpPost]
        public IActionResult Update(EditUser editUser)
        {
            var isModelValid = _accountsService.IsEditUserValid(ModelState, editUser);

            if (!isModelValid)
            {
                return View("Edit", editUser);
            }

            _accountsService.UpdateInfo(editUser);

            return RedirectToAction("Details", new { id = editUser.UserId });
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <returns>Admins users View</returns>
        /// <param name="id">Target user Id</param>  
        public IActionResult Delete(Guid id)
        {
            _accountsService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
