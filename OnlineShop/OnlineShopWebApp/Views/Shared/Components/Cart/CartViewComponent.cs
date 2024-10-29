using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly CartsService _cartsService;

        public CartViewComponent(CartsService cartsService)
        {
            _cartsService = cartsService;
        }

        /// <summary>
        /// Show cart icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public IViewComponentResult Invoke()
        {
            var cart = _cartsService.GetViewModel(_userId);
            var productsCount = cart?.TotalQuantity ?? 0;

            return View("Cart", productsCount);
        }
    }
}
