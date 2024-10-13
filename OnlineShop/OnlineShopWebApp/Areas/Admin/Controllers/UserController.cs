using Microsoft.AspNetCore.Mvc;
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
        /// Open Edit Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        /// <param name="id">Target user Id</param>
        public IActionResult Edit(Guid id)
        {
            var user = _accountsService.Get(id);
            return View(user);
        }

        /// <summary>
        /// Update target user
        /// </summary>
        /// <returns>Admin Users View if success; otherwise Edit User View</returns>
        [HttpPost]
        public IActionResult Update(User user)
        {
            return RedirectToAction("Index");
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
