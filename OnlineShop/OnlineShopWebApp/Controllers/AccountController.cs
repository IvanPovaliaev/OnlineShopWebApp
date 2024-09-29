using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Controllers
{
    public class AccountController : Controller
    {

        /// <summary>
        /// Login as user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public IActionResult Login(Login login, bool keepMeLogged)
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public IActionResult Register(Register register)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
