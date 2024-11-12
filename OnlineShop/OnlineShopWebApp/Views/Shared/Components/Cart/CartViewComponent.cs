using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private readonly string? _userId;
        private readonly CartsService _cartsService;

        public CartViewComponent(CartsService cartsService, IHttpContextAccessor httpContextAccessor)
        {
            _cartsService = cartsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show cart icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await _cartsService.GetViewModelAsync(_userId!);
            var productsCount = cart?.TotalQuantity ?? 0;

            return View("Cart", productsCount);
        }
    }
}
