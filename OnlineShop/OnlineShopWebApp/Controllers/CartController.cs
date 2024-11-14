using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly string? _userId;
        private readonly CartsService _cartsService;
        private readonly CookieCartsService _cookiesCartService;

        public CartController(CartsService cartsService, CookieCartsService cookieCartsService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _cookiesCartService = cookieCartsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        /// <summary>
        /// Get current cart for related user
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var cart = await _cartsService.GetViewModelAsync(_userId!);
                return View(cart);
            }

            var cookieCart = await _cookiesCartService.GetViewModelAsync();
            return View(cookieCart);

        }

        /// <summary>
        /// Add product to users cart
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (guid)</param>
        public async Task<IActionResult> Add(Guid productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _cartsService.AddAsync(productId, _userId);
            }
            else
            {
                await _cookiesCartService.AddAsync(productId);
            }

            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Increase quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Increase(Guid positionId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _cartsService.IncreasePositionAsync(_userId!, positionId);
            }
            else
            {
                await _cookiesCartService.IncreasePositionAsync(positionId);
            }

            return RedirectToAction("Index");

        }

        /// <summary>
        /// Decrease quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Decrease(Guid positionId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _cartsService.DecreasePosition(_userId, positionId);
            }
            else
            {
                await _cookiesCartService.DecreasePosition(positionId);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Delete()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _cartsService.DeleteAsync(_userId);
            }
            else
            {
                _cookiesCartService.Delete();
            }


            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete target position by Id
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> DeletePosition(Guid positionId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _cartsService.DeletePositionAsync(_userId!, positionId);
            }
            else
            {
                await _cookiesCartService.DeletePositionAsync(positionId);
            }

            return RedirectToAction("Index");
        }
    }
}
