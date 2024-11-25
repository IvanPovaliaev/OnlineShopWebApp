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
        private readonly IMailService _mailService;

        public AccountController(IAccountsService accountService, IOrdersService ordersService, IMediator mediator, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _accountsService = accountService;
            _ordersService = ordersService;
            _mediator = mediator;
            _mailService = mailService;
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
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var orders = await _ordersService.GetAllAsync(_userId!);
            return View(orders);
        }

        /// <summary>
        /// Login as user
        /// </summary>
        /// <returns>Initial view</returns>
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
        /// <returns>Initial view</returns>
        public async Task<IActionResult> Logout()
        {
            await _accountsService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns>Initial view</returns>
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

        /// <summary>
        /// Starts the password reset process
        /// </summary>
        /// <returns>SendResetPassword url</returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var isModelValid = await _accountsService.IsForgotPasswordValidAsync(ModelState, model);

            if (!isModelValid)
            {
                return PartialView("_ForgotPasswordForm", model);
            }

            var token = await _accountsService.GetPasswordResetTokenAsync(model.Email);

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                model.Email,
                Token = token
            }, protocol: HttpContext.Request.Scheme);

            await _mailService.SendEmailAsync(model.Email, "Сброс пароля",
    $"Ваш e-mail был указан для восстановления пароля в онлайн магазине PCDream.<br/>Для восстановления пароля перейдите, пожалуйста, <a href='{callbackUrl}'>по ссылке</a>");

            var redirectUrl = Url.Action(nameof(SendResetPassword));

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Open SendResetPassword view
        /// </summary>
        /// <returns>SendResetPassword view</returns>
        public IActionResult SendResetPassword() => View();

        /// <summary>
        /// Open ResetPassword view
        /// </summary>
        /// <returns>ResetPassword view</returns>
        public IActionResult ResetPassword(string email, string token)
        {
            var resetModel = new ResetPasswordViewModel()
            {
                Email = email,
                Token = token
            };

            return View(resetModel);
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <returns>SuccessResetPassword View</returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var isModelValid = await _accountsService.IsResetPasswordValidAsync(ModelState, model);

            if (!isModelValid)
            {
                return View(model);
            }

            var result = await _accountsService.TryResetPassword(model);

            if (!result)
            {
                return RedirectToAction("Error", "Home");
            }

            await _mailService.SendEmailAsync(model.Email, "Сброс пароля",
$"Ваш пароль был успешно изменён.");
            return RedirectToAction(nameof(SuccessResetPassword));
        }

        /// <summary>
        /// Open SuccessResetPassword view
        /// </summary>
        /// <returns>SuccessResetPassword view</returns>
        public IActionResult SuccessResetPassword() => View();
    }
}
