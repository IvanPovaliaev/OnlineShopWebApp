using OnlineShop.Application.Models;
using System;
using System.Threading.Tasks;

namespace OnlineShop.Application.Interfaces
{
    public interface ICookieCartsService
    {
        /// <summary>
        /// Get cart from Cookie
        /// </summary>        
        /// <returns>CartViewModel from Cookie</returns>
        Task<CartViewModel> GetViewModelAsync();

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        Task AddAsync(Guid productId);

        /// <summary>
        /// Increase quantity of cookie cart position by 1
        /// </summary>        
        /// <param name="positionId">Id of cart position</param>
        Task IncreasePositionAsync(Guid positionId);

        /// <summary>
        /// Decrease position quantity in cookie cart by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="positionId">Id of cart position</param>
        Task DecreasePositionAsync(Guid positionId);

        /// <summary>
        /// Delete cookie cart
        /// </summary>
        void Delete();

        /// <summary>
        /// Delete target position in cookie users
        /// </summary>
        /// <param name="positionId">Target positionId</param>
        Task DeletePositionAsync(Guid positionId);
    }
}
