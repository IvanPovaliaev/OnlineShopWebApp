using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly CartsService _cartsService;
        private readonly OrdersService _ordersService;

        public OrderController(CartsService cartsService, OrdersService ordersService)
        {
            _cartsService = cartsService;
            _ordersService = ordersService;
        }

        /// <summary>
        /// Open Admin Orders Page
        /// </summary>
        /// <returns>Admin Orders View</returns>
        public IActionResult Index()
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
        public IActionResult UpdateStatus(Guid orderId, OrderStatus status)
        {
            _ordersService.UpdateStatus(orderId, status);
            return RedirectToAction("Index");
        }
    }
}
