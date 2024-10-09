using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductsService _productsService;
        private readonly OrdersService _ordersService;

        public AdminController(ProductsService productsService, OrdersService ordersService)
        {
            _productsService = productsService;
            _ordersService = ordersService;
        }

        /// <summary>
        /// Open Admin Orders Page
        /// </summary>
        /// <returns>Admin Orders View</returns>
        public IActionResult Orders()
        {
            var orders = _ordersService.GetAll();
            return View(orders);
        }

        /// <summary>
        /// Update target order status if possible
        /// </summary>
        /// <returns>Admin Orders View</returns>
        /// <param name="orderId">Order id (guid)</param>
        /// <param name="status">New order status</param>
        public IActionResult UpdateOrderStatus(Guid orderId, OrderStatus status)
        {
            _ordersService.UpdateStatus(orderId, status);
            return RedirectToAction("Orders");
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
        /// <returns>Admin Products View</returns>
        public IActionResult Products()
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
    }
}
