using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Infrastructure.Jwt;

namespace OnlineShop.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountsService _accountsService;
        private readonly JwtProvider _jwtProvider;

        public AccountController(IAccountsService accountsService, JwtProvider jwtProvider)
        {
            _accountsService = accountsService;
            _jwtProvider = jwtProvider;
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

            var token = _jwtProvider.GenerateToken(login.Email);

            return Ok(new { Token = token, Message = "Success" });
        }
    }
}
