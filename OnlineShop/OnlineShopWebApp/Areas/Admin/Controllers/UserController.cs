using Microsoft.AspNetCore.Mvc;
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
