using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class ComparisonController : Controller
	{
		private readonly string? _userId;
		private readonly IComparisonsService _comparisonsService;

		public ComparisonController(IComparisonsService comparisonsService, IHttpContextAccessor httpContextAccessor)
		{
			_comparisonsService = comparisonsService;
			_userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		/// <summary>
		/// Get all comparisons
		/// </summary>
		/// <returns>ILookup comparison grouped by product categories</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var comparisonsGroups = await _comparisonsService.GetGroupsAsync(_userId!);

			return Ok(comparisonsGroups);
		}

		/// <summary>
		/// Add product to users comparisons
		/// </summary>
		/// <returns>Operation StatusCode/returns>
		/// <param name="productId">Product id (GUID)</param>
		[HttpPost(nameof(Add))]
		public async Task<IActionResult> Add(Guid productId)
		{
			await _comparisonsService.CreateAsync(productId, _userId!);
			return Ok($"Product {productId} added to user {_userId} comparisons successfully");
		}

		/// <summary>
		/// Delete ComparisonProducts from users comparisons by Id.
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">ComparisonProduct id (GUID)</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await _comparisonsService.DeleteAsync(id);
			return Ok($"ComparisonProduct {id} deleted from user {_userId} comparisons successfully");
		}

		/// <summary>
		/// Delete all ComparisonProducts from users comparisons
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		[HttpDelete("all")]
		public async Task<IActionResult> DeleteAll()
		{
			await _comparisonsService.DeleteAllAsync(_userId!);
			return Ok($"All ComparisonProducts was be deleted from user {_userId} comparisons successfully");
		}
	}
}
