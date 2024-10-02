using System;

namespace OnlineShopWebApp.Services
{
    public class ProductsEventService
    {
        public event Action<Guid> ProductDeleted;

        public void OnProductDeleted(Guid productId)
        {
            ProductDeleted?.Invoke(productId);
        }
    }
}
