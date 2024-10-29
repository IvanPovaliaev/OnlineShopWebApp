using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductsService _productsService;

        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <returns>Product page View</returns>
        /// <param name="id">Product id (guid)</param>
        public IActionResult Index(Guid id)
        {
            var product = _productsService.GetViewModel(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Get all products of the target category.
        /// </summary>
        /// <returns>Page View of all products of the specified category</returns>
        /// <param name="category">Product category</param>        
        public IActionResult Category(ProductCategoriesViewModel category)
        {
            var products = _productsService.GetAll(category);
            var productsWithCategory = (products, category);
            return View(productsWithCategory);
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Page View of all products</returns>        
        public IActionResult All()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        /// <summary>
        /// Get all products that match the search query
        /// </summary>
        /// <returns>Page with found products</returns>        
        public IActionResult SearchResult(string searchQuery)
        {
            var products = _productsService.GetAllFromSearch(searchQuery);
            var productsWithQuery = (products, searchQuery);
            return View(productsWithQuery);
        }
    }
}