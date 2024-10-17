using MediatR;
using System;

namespace OnlineShopWebApp.Helpers.Notifications
{
    public class ProductDeletedNotification : INotification
    {
        public Guid ProductId { get; }

        public ProductDeletedNotification(Guid productId)
        {
            ProductId = productId;
        }
    }
}
