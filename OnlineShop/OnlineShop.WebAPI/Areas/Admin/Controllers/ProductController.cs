using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain;
using OnlineShop.WebAPI.Helpers;

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
		/// Add a new product
		/// </summary>
		/// <returns>Admins products View</returns> 
		/// <param name="product">Target product</param>
		[HttpPost("Add")]
		public async Task<IActionResult> Add([FromBody] AddProductViewModel product)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			await _productsService.AddAsync(product);

			var result = new
			{
				Message = $"New product added successfully"
			};

			return Ok(result);
		}

		/// <summary>
		/// Update target product
		/// </summary>
		/// <returns>Admins products View</returns>
		[HttpPost("Update")]
		public async Task<IActionResult> Update([FromBody] EditProductViewModel product)
		{
			var isModelValid = await _productsService.IsUpdateValidAsync(ModelState, product);

			if (!isModelValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			await _productsService.UpdateAsync(product);

			var result = new
			{
				Message = $"Product ({product.Id}) updated successfully"
			};

			return Ok(result);
		}

		/// <summary>
		/// Delete product by Id
		/// </summary>
		/// <returns>Admins products View</returns>
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
