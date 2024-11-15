using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private readonly string? _userId;
        private readonly CartsService _cartsService;
        private readonly CookieCartsService _cookieCartService;

        public CartViewComponent(CartsService cartsService, CookieCartsService cookieCartsService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _cookieCartService = cookieCartsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show cart icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            CartViewModel cart;
            if (User.Identity.IsAuthenticated)
            {
                cart = await _cartsService.GetViewModelAsync(_userId!);
            }
            else
            {
                cart = await _cookieCartService.GetViewModelAsync();
            }

            var productsCount = cart?.TotalQuantity ?? 0;

            return View("Cart", productsCount);
        }
    }
}
