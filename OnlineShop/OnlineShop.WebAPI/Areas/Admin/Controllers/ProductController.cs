using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Domain;

namespace OnlineShop.WebAPI.Areas.Admin.Controllers
{
	[ApiController]
	[Route("[area]/[controller]")]
	[Area(Constants.AdminRoleName)]
	[Authorize(Roles = Constants.AdminRoleName)]
	public class ProductController : Controller
	{
		private readonly IProductsService _productsService;

		public ProductController(IProductsService productsService)
		{
			_productsService = productsService;
		}

		/// <summary>
		/// Delete product by Id
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="id">Target productId</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await _productsService.DeleteAsync(id);

			var result = new
			{
				Message = $"Product ({id}) deleted successfully"
			};

			return Ok(result);
		}
	}
}
