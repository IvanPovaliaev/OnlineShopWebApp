using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;

namespace OnlineShop.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ProductController : Controller
	{
		private readonly IProductsService _productsService;

		public ProductController(IProductsService productsService)
		{
			_productsService = productsService;
		}

		/// <summary>
		/// Get product by id
		/// </summary>
		/// <returns>ProductViewModel</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(Guid id)
		{
			var product = await _productsService.GetViewModelAsync(id);

			return product is null ? NotFound() : Ok(product);
		}

		/// <summary>
		/// Get all products of the target category.
		/// </summary>
		/// <returns>Collection of ProductViewModels of target category</returns>
		/// <param name="category">Product category</param>
		[HttpGet("Category")]
		public async Task<IActionResult> Get(ProductCategoriesViewModel category)
		{
			var products = await _productsService.GetAllAsync(category);
			return Ok(products);
		}

		/// <summary>
		/// Get all products
		/// </summary>
		/// <returns>Collection of ProductViewModels</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var products = await _productsService.GetAllAsync();
			return Ok(products);
		}

		/// <summary>
		/// Get all products that match the search query
		/// </summary>
		/// <returns>Collection of all products that match the search query</returns> 
		[HttpGet("search/{searchQuery}")]
		public async Task<IActionResult> GetFromSearchQuery(string searchQuery)
		{
			var products = await _productsService.GetAllFromSearchAsync(searchQuery);
			return products.Count != 0 ? Ok(products) : NotFound();
		}
	}
}