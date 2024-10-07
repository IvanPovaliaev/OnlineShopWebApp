using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;

namespace OnlineShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

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
            var isModelValid = _accountService.IsRegisterValid(ModelState, register);

            if (!isModelValid)
            {
                return PartialView("_RegistrationForm", register);
            }

            var redirectUrl = Url.Action("Index", "Home");

            return Json(new { redirectUrl });
        }
    }
}
