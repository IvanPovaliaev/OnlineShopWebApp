using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
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
			await _favoritesService.CreateAsync(productId, _userId!);
			var result = new
			{
				Message = $"Product {productId} added to user {_userId} favorites successfully"
			};
			return Ok(result);
		}

		/// <summary>
		/// Delete product from users favorites
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">FavoriteProduct Id (GUID)</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await _favoritesService.DeleteAsync(id);

			var result = new
			{
				Message = $"FavoriteProduct {id} deleted from user {_userId} comparisons successfully"
			};

			return Ok(result);
		}

		[HttpDelete("All")]
		/// <summary>
		/// Delete all FavoriteProducts by userId
		/// </summary>
		/// <return>sOperation StatusCode</returns>
		public async Task<IActionResult> DeleteAll()
		{
			await _favoritesService.DeleteAllAsync(_userId!);

			var result = new
			{
				Message = $"All FavoriteProducts was be deleted from user {_userId} comparisons successfully"
			};

			return Ok(result);
		}
	}
}
