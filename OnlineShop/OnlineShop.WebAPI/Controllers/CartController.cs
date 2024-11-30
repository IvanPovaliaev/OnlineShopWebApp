using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using System.Security.Claims;

namespace OnlineShop.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly string? _userId;
        private readonly ICartsService _cartsService;

        public CartController(ICartsService cartsService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _userId = User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Get current cart for related user
        /// </summary>
        /// <returns>Users CartViewModel</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cart = await _cartsService.GetViewModelAsync(_userId!);

            return cart is null ? NoContent() : Ok(cart);
        }

        /// <summary>
        /// Add product to users cart
        /// </summary>
        /// <returns>Operation StatusCode</returns>
        /// <param name="productId">Product id (guid)</param>
        [HttpPost("add")]
        public async Task<IActionResult> Add(Guid productId)
        {
            await _cartsService.AddAsync(productId, _userId!);

            return Ok();
        }

        /// <summary>
        /// Increase quantity of target position
        /// </summary>
        /// <returns>Operation StatusCode</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        [HttpPost("increase")]
        public async Task<IActionResult> Increase(Guid positionId)
        {
            await _cartsService.IncreasePositionAsync(_userId!, positionId);

            return Ok();

        }

        /// <summary>
        /// Decrease quantity of target position
        /// </summary>
        /// <returns>Operation StatusCode</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        [HttpPost("decrease")]
        public async Task<IActionResult> Decrease(Guid positionId)
        {
            await _cartsService.DecreasePositionAsync(_userId!, positionId);

            return Ok();
        }

        /// <summary>
        /// Delete users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _cartsService.DeleteAsync(_userId!);
            return Ok();
        }

        /// <summary>
        /// Delete target position by Id
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        [HttpDelete("position")]
        public async Task<IActionResult> DeletePosition(Guid positionId)
        {
            await _cartsService.DeletePositionAsync(_userId!, positionId);

            return Ok();
        }
    }
}
