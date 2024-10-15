using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;

namespace OnlineShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountsService _accountsService;

        public AccountController(AccountsService accountService)
        {
            _accountsService = accountService;
        }

        /// <summary>
        /// Login as user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public IActionResult Login(Login login, bool keepMeLogged)
        {
            var isModelValid = _accountsService.IsLoginValid(ModelState, login);

            if (!isModelValid)
            {
                return PartialView("_LoginForm", login);
            }

            var redirectUrl = Url.Action("Index", "Home");

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public IActionResult Register(UserRegister register)
        {
            var isModelValid = _accountsService.IsRegisterValid(ModelState, register);

            if (!isModelValid)
            {
                return PartialView("_RegistrationForm", register);
            }

            _accountsService.Add(register);

            var redirectUrl = Url.Action("Index", "Home");

            return Json(new { redirectUrl });
        }
    }
}
