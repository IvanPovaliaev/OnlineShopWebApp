using MediatR;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class ComparisonsOnProductDeletedHandler(ComparisonsService comparisonsService) : INotificationHandler<ProductDeletedNotification>
    {
        private readonly ComparisonsService _comparisonsService = comparisonsService;

        public async Task Handle(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            _comparisonsService.DeleteAllByProductId(notification.ProductId);
        }
    }
}
