using MediatR;
using OnlineShopWebApp.Models.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models.Handlers
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
