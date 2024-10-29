using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Create(UserDeliveryInfoViewModel deliveryInfo)
        {
            var cart = await _cartsService.GetAsync(_userId);
            var positions = cart.Positions;

            var isModelValid = _ordersService.IsCreationValid(ModelState, positions);

            if (!isModelValid)
            {
                return BadRequest();
            }

            await _ordersService.CreateAsync(_userId, deliveryInfo, positions);
            await _cartsService.DeleteAsync(_userId);

            var order = await _ordersService.GetLastAsync(_userId);

            return View(order);
        }
    }
}
