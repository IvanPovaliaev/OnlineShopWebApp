using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Db;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class OrderController : Controller
    {
        private readonly IOrdersService _ordersService;

        public OrderController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        /// <summary>
        /// Open Admin Orders Page
        /// </summary>
        /// <returns>Admin Orders View</returns>
        public async Task<IActionResult> Index()
        {
            var orders = await _ordersService.GetAllAsync();
            return View(orders);
        }

        /// <summary>
        /// Update target order status if possible
        /// </summary>
        /// <returns>Admin Orders View</returns>
        /// <param name="id">Order id (guid)</param>
        /// <param name="status">New order status</param>
        public async Task<IActionResult> UpdateStatus(Guid id, OrderStatusViewModel status)
        {
            await _ordersService.UpdateStatusAsync(id, status);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Export all roles info to excel
        /// </summary>
        /// <returns>Excel file with roles info</returns>
        public async Task<IActionResult> ExportToExcel()
        {
            var stream = await _ordersService.ExportAllToExcelAsync();

            var downloadFileStream = new FileStreamResult(stream, Formats.ExcelFileContentType)
            {
                FileDownloadName = "Orders.xlsx"
            };

            return downloadFileStream;
        }
    }
}
