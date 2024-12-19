using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Infrastructure.ReviewApiService;

namespace OnlineShop.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly IReviewsService _reviewService;

        public ProductController(IProductsService productsService, IReviewsService reviewService)
        {
            _productsService = productsService;
            _reviewService = reviewService;
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
        /// Get product by id with all reviews
        /// </summary>
        /// <returns>Model that contains product and its reviews</returns>
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetProductWithReviews(Guid id)
        {
            var product = await _productsService.GetViewModelAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            var productWithReview = new
            {
                Product = product,
                Reviews = reviews
            };

            return Ok(productWithReview);
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
        [HttpGet("search/{query}")]
        public async Task<IActionResult> GetFromSearchQuery(string query)
        {
            var products = await _productsService.GetAllFromSearchAsync(query);
            return products.Count != 0 ? Ok(products) : NotFound();
        }
    }
}