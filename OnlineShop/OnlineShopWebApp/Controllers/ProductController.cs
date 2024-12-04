using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Infrastructure.ApiServices;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductsService productsService, IReviewService reviewService)
        {
            _productsService = productsService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <returns>Product page View</returns>
        /// <param name="id">Product id (guid)</param>
        public async Task<IActionResult> Index(Guid id)
        {
            var product = await _productsService.GetViewModelAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            var productWithReview = (product, reviews);

            return View(productWithReview);
        }

        /// <summary>
        /// Get all products of the target category.
        /// </summary>
        /// <returns>Page View of all products of the specified category</returns>
        /// <param name="category">Product category</param>        
        public async Task<IActionResult> Category(ProductCategoriesViewModel category)
        {
            var products = await _productsService.GetAllAsync(category);
            var productsWithCategory = (products, category);
            return View(productsWithCategory);
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Page View of all products</returns>        
        public async Task<IActionResult> All()
        {
            var products = await _productsService.GetAllAsync();
            return View(products);
        }

        /// <summary>
        /// Get all products that match the search query
        /// </summary>
        /// <returns>Page with found products</returns>        
        public async Task<IActionResult> SearchResult(string searchQuery)
        {
            var products = await _productsService.GetAllFromSearchAsync(searchQuery);
            var productsWithQuery = (products, searchQuery);
            return View(productsWithQuery);
        }

        /// <summary>
        /// Add a new review for product
        /// </summary>
        /// <returns>Product page view</returns>        
        public async Task<IActionResult> AddReview(AddReviewViewModel newReview)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddReviewForm", newReview);
            }

            var isSuccess = await _reviewService.AddReviewAsync(newReview);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при отправке отзыва. Попробуйте позднее.");
                return PartialView("_AddReviewForm", newReview);
            }

            var redirectUrl = Url.Action(nameof(Index), new { id = newReview.ProductId });

            return Json(new { redirectUrl });
        }

    }
}