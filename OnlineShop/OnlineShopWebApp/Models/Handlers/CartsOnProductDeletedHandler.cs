using MediatR;
using OnlineShopWebApp.Models.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models.Handlers
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
