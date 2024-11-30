using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.WebAPI.Helpers;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly string _userId;
        private readonly ICartsService _cartsService;
        private readonly IOrdersService _ordersService;

        public OrderController(ICartsService cartsService, IOrdersService ordersService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _ordersService = ordersService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        /// <summary>
        /// Create user order
        /// </summary>
        /// <returns>Create order view</returns>
        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create(UserDeliveryInfoViewModel deliveryInfo)
        {
            var cart = await _cartsService.GetAsync(_userId);
            var positions = cart?.Positions;

            var isModelValid = _ordersService.IsCreationValid(ModelState, positions!);

            if (!isModelValid)
            {
                return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
            }

            await _ordersService.CreateAsync(_userId, deliveryInfo, positions);
            await _cartsService.DeleteAsync(_userId);

            var order = await _ordersService.GetLastAsync(_userId);

            return Ok(order);
        }
    }
}
