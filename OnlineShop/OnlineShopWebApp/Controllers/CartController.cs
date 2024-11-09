using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly CartsService _cartsService;

        public CartController(CartsService cartsService)
        {
            _cartsService = cartsService;
        }

        /// <summary>
        /// Get current cart for related user
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Index()
        {
            var cart = await _cartsService.GetViewModelAsync(_userId);
            return View(cart);
        }

        /// <summary>
        /// Add product to users cart
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (guid)</param>
        public async Task<IActionResult> Add(Guid productId)
        {
            await _cartsService.AddAsync(productId, _userId);
            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Increase quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Increase(Guid positionId)
        {
            await _cartsService.IncreasePositionAsync(_userId, positionId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Decrease quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> Decrease(Guid positionId)
        {
            await _cartsService.DecreasePosition(_userId, positionId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        public async Task<IActionResult> Delete()
        {
            await _cartsService.DeleteAsync(_userId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete target position by Id
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public async Task<IActionResult> DeletePosition(Guid positionId)
        {
            await _cartsService.DeletePositionAsync(_userId, positionId);
            return RedirectToAction("Index");
        }
    }
}
