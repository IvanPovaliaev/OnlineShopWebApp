using MediatR;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class CartsOnProductDeletedHandler(CartsService cartsService) : INotificationHandler<ProductDeletedNotification>
    {
        private readonly CartsService _cartsService = cartsService;

        public async Task Handle(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            _cartsService.DeletePositionsByProductId(notification.ProductId);
        }
    }
}
