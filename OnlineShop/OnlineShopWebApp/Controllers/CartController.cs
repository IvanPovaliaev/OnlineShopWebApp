using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly string? _userId;
        private readonly ICartsService _cartsService;
        private readonly ICookieCartsService _cookiesCartService;
        private readonly AuthenticationHelper _authenticationHelper;

        public CartController(ICartsService cartsService, ICookieCartsService cookieCartsService, IHttpContextAccessor httpContextAccessor, AuthenticationHelper authenticationHelper)
        {
            _cartsService = cartsService;
            _cookiesCartService = cookieCartsService;
            _authenticationHelper = authenticationHelper;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        /// <summary>
        /// Get current cart for related user
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Index()
        {
            var cart = await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                () => _cartsService.GetViewModelAsync(_userId!),
                _cookiesCartService.GetViewModelAsync);

            return View(cart);
        }

        /// <summary>
        /// Add product to users cart
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (guid)</param>
        public async Task<IActionResult> Add(Guid productId)
        {
            await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                () => _cartsService.AddAsync(productId, _userId!),
                () => _cookiesCartService.AddAsync(productId));

            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Increase quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Increase(Guid positionId)
        {
            await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                () => _cartsService.IncreasePositionAsync(_userId!, positionId),
                () => _cookiesCartService.IncreasePositionAsync(positionId)
            );

            return RedirectToAction(nameof(Index));

        }

        /// <summary>
        /// Decrease quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Decrease(Guid positionId)
        {
            await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                    () => _cartsService.DecreasePositionAsync(_userId!, positionId),
                    () => _cookiesCartService.DecreasePositionAsync(positionId)
                );

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Delete()
        {
            await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                    async () => await _cartsService.DeleteAsync(_userId!),
                    _cookiesCartService.Delete
                );

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete target position by Id
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> DeletePosition(Guid positionId)
        {
            await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                () => _cartsService.DeletePositionAsync(_userId!, positionId),
                () => _cookiesCartService.DeletePositionAsync(positionId)
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
