using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
	/// <summary>
	/// Controller for managing users carts.
	/// </summary>
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly string? _userId;
		private readonly ICartsService _cartsService;

		public CartController(ICartsService cartsService, IHttpContextAccessor httpContextAccessor)
		{
			_cartsService = cartsService;
			_userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
		}

		/// <summary>
		/// Get current cart for related user
		/// </summary>
		/// <returns>NoContent if cart not exist; otherwise return cart view model</returns>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var cart = await _cartsService.GetViewModelAsync(_userId!);

			return cart is null ? NoContent() : Ok(cart);
		}

		/// <summary>
		/// Add product to users cart
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="productId">Product id (guid)</param>
		[HttpPost(nameof(Add))]
		public async Task<IActionResult> Add(Guid productId)
		{
			var isSuccess = await _cartsService.AddAsync(productId, _userId!);
			if (isSuccess)
			{
				return Ok(new { Message = $"Product {productId} added successfully to user cart" });
			}

			return NotFound(new { Message = $"Product {productId} not found" });
		}

		/// <summary>
		/// Increase quantity of target position
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="positionId">Position ID (GUID)</param>
		[HttpPost(nameof(Increase))]
		public async Task<IActionResult> Increase(Guid positionId)
		{
			var isSuccess = await _cartsService.IncreasePositionAsync(_userId!, positionId);
			if (isSuccess)
			{
				return Ok(new { Message = $"Position {positionId} increased successfully" });
			}
			return NotFound(new { Message = $"Position {positionId} not found" });
		}

		/// <summary>
		/// Decrease quantity of target position
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="positionId">Position ID (GUID)</param>
		[HttpPost(nameof(Decrease))]
		public async Task<IActionResult> Decrease(Guid positionId)
		{
			var isSuccess = await _cartsService.DecreasePositionAsync(_userId!, positionId);
			if (isSuccess)
			{
				return Ok(new { Message = $"Position {positionId} decreased successfully" });
			}
			return NotFound(new { Message = $"Position {positionId} not found" });
		}

		/// <summary>
		/// Delete users cart
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		[HttpDelete]
		public async Task<IActionResult> Delete()
		{
			var isSuccess = await _cartsService.DeleteAsync(_userId!);
			if (isSuccess)
			{
				return Ok(new { Message = $"Cart for user {_userId} deleted successfully" });
			}

			return NotFound(new { Message = $"No Cart found for user {_userId}." });
		}

		/// <summary>
		/// Delete target position by Id
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">Position ID (GUID)</param>
		[HttpDelete("Position/{id}")]
		public async Task<IActionResult> DeletePosition(Guid id)
		{
			var isSuccess = await _cartsService.DeletePositionAsync(_userId!, id);
			if (isSuccess)
			{
				return Ok(new { Message = $"Position {id} deleted successfully" });
			}

			return NotFound(new { Message = $"Position {id} not found" });
		}
	}
}
