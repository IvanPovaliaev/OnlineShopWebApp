﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.DTO;
using OnlineShop.Infrastructure.Jwt;
using OnlineShop.WebAPI.Helpers;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
    /// <summary>
    /// Controller for users accounts.
    /// </summary>
    [ApiController]
	[Route("[controller]")]
	public class AccountController : Controller
	{
		private readonly string? _userId;
		private readonly IAccountsService _accountsService;
		private readonly IOrdersService _ordersService;
		private readonly JwtProvider _jwtProvider;

		public AccountController(IAccountsService accountsService, IOrdersService ordersService, IHttpContextAccessor httpContextAccessor, JwtProvider jwtProvider)
		{
			_accountsService = accountsService;
			_ordersService = ordersService;
			_jwtProvider = jwtProvider;
			_userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
		}

		/// <summary>
		/// Login as user
		/// </summary>
		/// <returns>Authentication token</returns>
		[HttpPost(nameof(Login))]
		public async Task<IActionResult> Login([FromBody] LoginDTO login)
		{
			var isModelValid = await _accountsService.IsLoginValidAsync(ModelState, login);

			if (!isModelValid)
			{
				return Unauthorized(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			var token = _jwtProvider.GenerateToken(login.Email);

			return Ok(new { Token = token, Message = "Success" });
		}

		/// <summary>
		/// Register a new user
		/// </summary>
		/// <returns>Authentication token</returns>
		[HttpPost(nameof(Register))]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel register)
		{
			var isModelValid = await _accountsService.IsRegisterValidAsync(ModelState, register);

			if (!isModelValid)
			{
				return Unauthorized(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			await _accountsService.AddAsync(register);

			var token = _jwtProvider.GenerateToken(register.Email);

			return Ok(new { Token = token, Message = "New user registered successfully" });
		}

		/// <summary>
		/// Get current user model
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		[Authorize]
		[HttpGet("Info")]
		public async Task<IActionResult> GetInfo()
		{
			var user = await _accountsService.GetAsync(_userId!);
			return Ok(user);
		}

		/// <summary>
		/// Get all orders related to current user
		/// </summary>
		/// <returns>Collection of user orders</returns>
		[Authorize]
		[HttpGet(nameof(Orders))]
		public async Task<IActionResult> Orders()
		{
			var orders = await _ordersService.GetAllAsync(_userId!);
			return Ok(orders);
		}
	}
}
