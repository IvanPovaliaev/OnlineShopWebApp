using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain;

namespace OnlineShop.WebAPI.Areas.Admin.Controllers
{
	[ApiController]
	[Route("[area]/[controller]")]
	[Area(Constants.AdminRoleName)]
	[Authorize(Roles = Constants.AdminRoleName)]
	public class OrderController : Controller
	{
		private readonly IOrdersService _ordersService;

		public OrderController(IOrdersService ordersService)
		{
			_ordersService = ordersService;
		}

		/// <summary>
		/// Get all orders
		/// </summary>
		/// <returns>Collection of ordersViewModel</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var orders = await _ordersService.GetAllAsync();
			return Ok(orders);
		}

		/// <summary>
		/// Update target order status if possible
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">Order id (guid)</param>
		/// <param name="status">New order status</param>
		[HttpGet(nameof(UpdateStatus))]
		public async Task<IActionResult> UpdateStatus(Guid id, OrderStatusViewModel status)
		{
			await _ordersService.UpdateStatusAsync(id, status);

			var result = new
			{
				Message = $"Order ({id}) status was change to '{status}' successfully"
			};

			return Ok(result);
		}
	}
}
