﻿using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly OrdersService _ordersService;

        public OrderController(OrdersService ordersService)
        {
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
        /// <param name="id">Order id (guid)</param>
        /// <param name="status">New order status</param>
        public IActionResult UpdateStatus(Guid id, OrderStatus status)
        {
            _ordersService.UpdateStatus(id, status);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Export all roles info to excel
        /// </summary>
        /// <returns>Excel file with roles info</returns>
        public IActionResult ExportToExcel()
        {
            var stream = _ordersService.ExportAllToExcel();

            var downloadFileStream = new FileStreamResult(stream, Constants.ExcelFileContentType)
            {
                FileDownloadName = "Orders.xlsx"
            };

            return downloadFileStream;
        }
    }
}
