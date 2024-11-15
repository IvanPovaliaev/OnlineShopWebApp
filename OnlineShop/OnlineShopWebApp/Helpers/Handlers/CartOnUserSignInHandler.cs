using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Services;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class CartOnUserSignInHandler(IHttpContextAccessor httpContextAccessor, CartsService cartsService, CookieCartsService cookieCartsService) : INotificationHandler<UserSignInNotification>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor!;
        private readonly CartsService _cartsService = cartsService;
        private readonly CookieCartsService _cookieCartsService = cookieCartsService;

        public async Task Handle(UserSignInNotification notification, CancellationToken cancellationToken)
        {
            var cookieCart = await _cookieCartsService.GetViewModelAsync();
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _cartsService.ReplaceFromCookieAsync(cookieCart, userId);
            _cookieCartsService.Delete();
        }
    }
}
