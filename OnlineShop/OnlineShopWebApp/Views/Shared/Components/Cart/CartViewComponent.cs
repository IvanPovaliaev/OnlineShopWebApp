using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShopWebApp.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private readonly string? _userId;
        private readonly ICartsService _cartsService;
        private readonly ICookieCartsService _cookieCartsService;
        private readonly AuthenticationHelper _authenticationHelper;

        public CartViewComponent(ICartsService cartsService, ICookieCartsService cookieCartsService, IHttpContextAccessor httpContextAccessor, AuthenticationHelper authenticationHelper)
        {
            _cartsService = cartsService;
            _cookieCartsService = cookieCartsService;
            _authenticationHelper = authenticationHelper;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show cart icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await _authenticationHelper.ExecuteBasedOnAuthenticationAsync(
                    () => _cartsService.GetViewModelAsync(_userId!),
                    _cookieCartsService.GetViewModelAsync);

            var productsCount = cart?.TotalQuantity ?? 0;

            return View("Cart", productsCount);
        }
    }
}
