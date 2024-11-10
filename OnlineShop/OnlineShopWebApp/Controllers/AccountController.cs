using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Threading.Tasks;

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
        /// Open unauthorized page
        /// </summary>
        /// <returns>Unauthorized page</returns>
        public IActionResult Unauthorized(string returnUrl)
        {
            return View();
        }

        /// <summary>
        /// Login as user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var isModelValid = await _accountsService.IsLoginValidAsync(ModelState, login);

            if (!isModelValid)
            {
                return PartialView("_LoginForm", login);
            }

            var redirectUrl = login.ReturnUrl;

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Logout user
        /// </summary>
        /// <returns>Home page</returns>
        public async Task<IActionResult> Logout()
        {
            await _accountsService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns>Home page</returns>
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel register)
        {
            var isModelValid = await _accountsService.IsRegisterValidAsync(ModelState, register);

            if (!isModelValid)
            {
                return PartialView("_RegistrationForm", register);
            }

            await _accountsService.AddAsync(register);

            var redirectUrl = register.ReturnUrl;

            return Json(new { redirectUrl });
        }
    }
}
