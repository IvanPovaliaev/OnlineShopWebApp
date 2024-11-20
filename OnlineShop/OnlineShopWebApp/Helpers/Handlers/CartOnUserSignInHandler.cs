using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class CartOnUserSignInHandler(IHttpContextAccessor httpContextAccessor, ICartsService cartsService, ICookieCartsService cookieCartsService) : INotificationHandler<UserSignInNotification>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ICartsService _cartsService = cartsService;
        private readonly ICookieCartsService _cookieCartsService = cookieCartsService;

        public async Task Handle(UserSignInNotification notification, CancellationToken cancellationToken)
        {
            var cookieCart = await _cookieCartsService.GetViewModelAsync();
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _cartsService.ReplaceFromCookieAsync(cookieCart, userId);
            _cookieCartsService.Delete();
        }
    }
}
