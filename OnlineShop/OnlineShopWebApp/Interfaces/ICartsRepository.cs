using OnlineShopWebApp.Models;
using System;

namespace OnlineShopWebApp.Interfaces
{
    public interface ICartsRepository
    {
        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        Cart Get(Guid userId);

        /// <summary>
        /// Create a new cart
        /// </summary> 
        /// <param name="cart">Target cart</param>
        void Create(Cart cart);

        /// <summary>
        /// Update cart with identical id. If cart is not in the repository - does nothing.
        /// </summary>
        /// <param name="cart">Target cart</param>
        void Update(Cart cart);

        /// <summary>
        /// Delete cart with identical id. If cart is not in the repository - does nothing.
        /// </summary>
        /// <param name="cart">Target cart</param>
        void Delete(Cart cart);

        /// <summary>
        /// Delete all CartPositions related to product Id.
        /// </summary>
        /// <param name="productId">Target product Id (guid)</param>
        void DeletePositionsByProductId(Guid productId);
    }
}
