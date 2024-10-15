using MediatR;
using OnlineShopWebApp.Models.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models.Handlers
{
    public class ProductDeletedHandler : INotificationHandler<ProductDeletedNotification>
    {
        private readonly ComparisonsService _comparisonsService;
        private readonly FavoritesService _favoritesService;
        private readonly CartsService _cartsService;

        public ProductDeletedHandler(ComparisonsService comparisonsService, FavoritesService favoritesService, CartsService cartsService)
        {
            _comparisonsService = comparisonsService;
            _favoritesService = favoritesService;
            _cartsService = cartsService;
        }

        public async Task Handle(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            _comparisonsService.DeleteAllByProductId(notification.ProductId);
            _favoritesService.DeleteAllByProductId(notification.ProductId);
            _cartsService.DeletePositionsByProductId(notification.ProductId);
        }
    }
}
