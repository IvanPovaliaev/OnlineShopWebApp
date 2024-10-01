using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductsService _productsService;

        public AdminController(ProductsService productsService)
        {
            _productsService = productsService;
        }
        /// <summary>
        /// Open Admin Orders Page
        /// </summary>
        /// <returns>Admin Orders View</returns>
        public IActionResult Orders()
        {
            return View();
        }

        /// <summary>
        /// Open Admin Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        public IActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// Open Admin Roles Page
        /// </summary>
        /// <returns>Admin Roles View</returns>
        public IActionResult Roles()
        {
            return View();
        }

        /// <summary>
        /// Open Admin Products Page
        /// </summary>
        /// <returns>Admin Producs View</returns>
        public IActionResult Products()
        {
            var products = _productsService.GetAll();
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult EditProduct(Guid productId)
        {
            var product = _productsService.Get(productId);
            return View(product);
        }

    }
}
