using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly OrdersService _ordersService;
        private readonly IExcelService _excelService;

        public OrderController(OrdersService ordersService, IExcelService excelService)
        {
            _ordersService = ordersService;
            _excelService = excelService;
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
        /// <param name="id">Order id (guid)</param>
        /// <param name="status">New order status</param>
        public IActionResult UpdateStatus(Guid id, OrderStatus status)
        {
            _ordersService.UpdateStatus(id, status);
            return RedirectToAction("Index");
        }

        public IActionResult ExportToExcel()
        {
            var orders = _ordersService.GetAll();
            var excelStream = _excelService.ExportOrders(orders);
            return File(excelStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }
    }
}
