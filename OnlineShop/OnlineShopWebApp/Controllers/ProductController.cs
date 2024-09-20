using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private ProductsService _productsService;

        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <returns>Product page View</returns>
        /// <param name="id">Product id (guid)</param>
        [Route("index/{id}")]
        public IActionResult Get(Guid id)
        {
            var product = _productsService.Get(id);
            return View(product);
        }

        /// <summary>
        /// Get all products of the target category.
        /// </summary>
        /// <returns>Page View of all products of the specified category</returns>
        /// <param name="category">Product category</param>
        [Route("categories/{category}")]
        public IActionResult GetByCategory(ProductCategories category)
        {
            var products = _productsService.GetAll(category);
            var productsWithCategory = (products, category);
            return View(productsWithCategory);
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Page View of all products</returns>
        [Route("index")]
        public IActionResult GetAll()
        {
            var products = _productsService.GetAll();
            return View(products);
        }
    }
}
