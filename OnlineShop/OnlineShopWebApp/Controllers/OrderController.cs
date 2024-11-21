using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        private string _userId;
        private readonly ICartsService _cartsService;
        private readonly IOrdersService _ordersService;

        public OrderController(ICartsService cartsService, IOrdersService ordersService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _ordersService = ordersService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
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
