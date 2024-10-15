using MediatR;
using System;

namespace OnlineShopWebApp.Models.Notifications
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
