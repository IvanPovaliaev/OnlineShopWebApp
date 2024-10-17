using MediatR;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class FavoritesOnProductDeletedHandler(FavoritesService favoritesService) : INotificationHandler<ProductDeletedNotification>
    {
        private readonly FavoritesService _favoritesService = favoritesService;

        public async Task Handle(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            _favoritesService.DeleteAllByProductId(notification.ProductId);
        }
    }
}
