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
        public IActionResult Create(UserDeliveryInfoViewModel deliveryInfo)
        {
            var positions = _cartsService.Get(_userId).Positions;

            var isModelValid = _ordersService.IsCreationValid(ModelState, positions);

            if (!isModelValid)
            {
                return BadRequest();
            }

            _ordersService.Create(_userId, deliveryInfo, positions);
            _cartsService.Delete(_userId);

            var order = _ordersService.GetLast(_userId);

            return View(order);
        }
    }
}
