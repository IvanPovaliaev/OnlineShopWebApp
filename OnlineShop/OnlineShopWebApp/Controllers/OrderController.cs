using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly CartsService _cartsService;
        private readonly OrdersService _ordersService;

        public OrderController(CartsService cartsService, OrdersService ordersService)
        {
            _cartsService = cartsService;
            _ordersService = ordersService;
        }

        /// <summary>
        /// Create user order
        /// </summary>
        /// <returns>Create order view</returns>
        [HttpPost]
        public IActionResult Create(Order order)
        {
            order.Positions = _cartsService
                .Get(_userId).Positions;

            var isModelValid = _ordersService.IsCreationValid(ModelState, order);

            if (!isModelValid)
            {
                return BadRequest();
            }

            order.UserId = _userId;

            _cartsService.Delete(_userId);
            _ordersService.Create(order);
            return View(order);
        }
    }
}
