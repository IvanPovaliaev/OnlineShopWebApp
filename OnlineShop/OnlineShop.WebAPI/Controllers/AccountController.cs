using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Helpers.Notifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;

namespace OnlineShop.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountsService _accountsService;
        private readonly IMediator _mediator;

        public AccountController(IAccountsService accountsService, IMediator mediator)
        {
            _accountsService = accountsService;
            _mediator = mediator;
        }

        /// <summary>
        /// Login as user
        /// </summary>
        /// <returns>Result status code</returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            var isModelValid = await _accountsService.IsLoginValidAsync(ModelState, login);

            if (!isModelValid)
            {
                return Unauthorized("Invalid username or password");
            }

            await _mediator.Publish(new UserSignInNotification());

            return Ok("Login successful");
        }
    }
}
