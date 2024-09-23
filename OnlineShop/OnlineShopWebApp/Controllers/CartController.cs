using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class CartController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private CartsService _cartsService;
        private ProductsService _productsService;

        public CartController(CartsService cartsService, ProductsService productsService)
        {
            _cartsService = cartsService;
            _productsService = productsService;
        }

        /// <summary>
        /// Get current cart for related user
        /// </summary>
        /// <returns>Users cart View</returns>
        public IActionResult Index()
        {
            var cart = _cartsService.Get(_userId);
            return View(cart);
        }

        /// <summary>
        /// Add product to users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="productId">Product id (guid)</param>
        public IActionResult Add(Guid productId)
        {
            var product = _productsService.Get(productId);
            _cartsService.Add(product, _userId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Increase quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public IActionResult Increase(Guid positionId)
        {
            var cart = _cartsService.Get(_userId);
            _cartsService.IncreasePosition(cart, positionId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Decrease quantity of target position
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="positionId">Position ID (GUID)</param>
        public IActionResult Decrease(Guid positionId)
        {
            _cartsService.DecreaseQuantity(_userId, positionId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete users cart
        /// </summary>
        /// <returns>Users cart View</returns>
        public IActionResult Delete()
        {
            _cartsService.Delete(_userId);
            return RedirectToAction("Index");
        }
    }
}
