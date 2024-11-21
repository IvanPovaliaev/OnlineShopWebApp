using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly string? _userId;
        private readonly IAccountsService _accountsService;
        private readonly IOrdersService _ordersService;
        private readonly IMediator _mediator;

        public AccountController(IAccountsService accountService, IOrdersService ordersService, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _accountsService = accountService;
            _ordersService = ordersService;
            _mediator = mediator;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
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
        /// Open Index account page for currentUser
        /// </summary>
        /// <returns>Account index page</returns>
        /// <param name="user">Target EditUser model</param>  
        [Authorize]
        public async Task<IActionResult> Index(EditUserViewModel user)
        {
            if (user.Id is null)
            {
                ModelState.Clear();
                user = await _accountsService.GetEditViewModelAsync(_userId!);
            }

            return View(user);
        }

        /// <summary>
        /// Update target user
        /// </summary>
        /// <returns>Account index page</returns>
        /// <param name="editUser">Target EditUser model</param>		
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(EditUserViewModel editUser)
        {
            var isModelValid = await _accountsService.IsEditUserValidAsync(ModelState, editUser);

            if (!isModelValid)
            {
                return View(nameof(Index), editUser);
            }

            await _accountsService.UpdateInfoAsync(editUser);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Open Orders page for current user
        /// </summary>
        /// <returns>Orders page</returns>
        /// <param name="user">Target EditUser model</param>  
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var orders = await _ordersService.GetAllAsync(_userId!);
            return View(orders);
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

            await _mediator.Publish(new UserSignInNotification());

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

            await _mediator.Publish(new UserSignInNotification());

            return Json(new { redirectUrl });
        }
    }
}
