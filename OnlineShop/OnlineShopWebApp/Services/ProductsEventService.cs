using System;

namespace OnlineShopWebApp.Services
{
    public class ProductsEventService
    {
        public event Action<Guid> ProductDeleted;

        /// <summary>
        /// Initialize ProductDeleted event with productId
        /// </summary> 
        /// <param name="productId">Target productId (GUID)</param>
        public void OnProductDeleted(Guid productId)
        {
            ProductDeleted?.Invoke(productId);
        }
    }
}
