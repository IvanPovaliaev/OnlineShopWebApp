using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
	/// <summary>
	/// Controller for managing favorites.
	/// </summary>
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class FavoriteController : Controller
	{
		private readonly string? _userId;
		private readonly IFavoritesService _favoritesService;

		public FavoriteController(IFavoritesService favoritesService, IHttpContextAccessor httpContextAccessor)
		{
			_favoritesService = favoritesService;
			_userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		/// <summary>
		/// Get all favorites
		/// </summary>
		/// <returns>Favorites collection</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var favorites = await _favoritesService.GetAllAsync(_userId!);
			return Ok(favorites);
		}

		/// <summary>
		/// Add product to users favorites
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="productId">Product id (GUID)</param>
		[HttpPost(nameof(Add))]
		public async Task<IActionResult> Add(Guid productId)
		{
			var createdId = await _favoritesService.CreateAsync(productId, _userId!);

			if (createdId is null)
			{
				var message = new { Message = $"Product {productId} is already in user {_userId} favorites or product not found." };
				return BadRequest(message);
			}

			return Ok($"Product {productId} added to user {_userId} favorites successfully with id {createdId}");
		}

		/// <summary>
		/// Delete product from users favorites
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">FavoriteProduct Id (GUID)</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var isSuccess = await _favoritesService.DeleteAsync(id);

			if (isSuccess)
			{
				return Ok($"FavoriteProduct {id} deleted from user {_userId} favorites successfully");
			}

			return NotFound($"FavoriteProduct with id {id} not found");
		}

		/// <summary>
		/// Delete all FavoriteProducts by userId
		/// </summary>
		/// <return>Operation StatusCode</return>
		[HttpDelete("All")]
		public async Task<IActionResult> DeleteAll()
		{
			var isSuccess = await _favoritesService.DeleteAllAsync(_userId!);

			if (isSuccess)
			{
				return Ok($"All FavoriteProducts was be deleted from user {_userId} comparisons successfully");
			}

			return NotFound($"No FavoriteProducts found for user {_userId}.");
		}
	}
}
