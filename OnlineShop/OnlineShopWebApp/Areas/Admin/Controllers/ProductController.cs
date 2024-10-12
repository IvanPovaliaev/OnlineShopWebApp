using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductsService _productsService;

        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Open Admin Products Page
        /// </summary>
        /// <returns>Admin Products View</returns>
        public IActionResult Index()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        /// <summary>
        /// Open Admin AddProduct Page
        /// </summary>
        /// <returns>Admin AddProduct View</returns>
        public IActionResult AddProduct()
        {
            return View();
        }

        /// <summary>
        /// Open Admin EditProduct Page
        /// </summary>
        /// <returns>Admin EditProduct View</returns>
        /// <param name="orderId">Product id (guid)</param>
        public IActionResult EditProduct(Guid productId)
        {
            var product = _productsService.Get(productId);
            return View(product);
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <returns>Admins products View</returns> 
        /// <param name="product">Target product</param>
        [HttpPost]
        public IActionResult Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View("AddProduct", product);
            }

            _productsService.Add(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete product by Id
        /// </summary>
        /// <returns>Admins products View</returns>
        /// <param name="productId">Target productId</param>  
        public IActionResult Delete(Guid productId)
        {
            _productsService.Delete(productId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Update target product
        /// </summary>
        /// <returns>Admins products View</returns>
        [HttpPost]
        public IActionResult Update(Product product)
        {
            var isModelValid = _productsService.IsUpdateValid(ModelState, product);

            if (!isModelValid)
            {
                return View("EditProduct", product);
            }

            _productsService.Update(product);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get ViewComponent SpecificationsForm for product specifications depending on the category
        /// </summary>
        /// <returns>Relevant SpecificationsFormViewComponent</returns>
        /// <param name="category">Product category</param>   
        public IActionResult GetSpecificationsForm(ProductCategories category)
        {
            var emptySpecifications = new Dictionary<string, string>();
            var specificationsWithCategory = (emptySpecifications, category);
            return ViewComponent("SpecificationsForm", specificationsWithCategory);
        }
    }
}
