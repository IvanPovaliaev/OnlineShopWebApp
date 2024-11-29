using OnlineShop.Application.Models;
using OnlineShop.Domain.Models;
using System;
using System.Threading.Tasks;

namespace OnlineShop.Application.Interfaces
{
    public interface ICartsService
    {
        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        Task<Cart> GetAsync(string userId);

        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>CartViewModel for related user</returns>
        /// <param name="userId">GUID user id</param>
        Task<CartViewModel> GetViewModelAsync(string userId);

        /// <summary>
        /// Add product to users cart.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        Task AddAsync(Guid productId, string userId);

        /// <summary>
        /// Increase quantity of user cart position by 1
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        /// <param name="positionId">Id of cart position</param>
        Task IncreasePositionAsync(string userId, Guid positionId);

        /// <summary>
        /// Decrease position quantity in users cart by 1. If quantity should become 0, deletes this position.
        /// </summary>        
        /// <param name="userId">UserId</param>
        /// <param name="positionId">Id of cart position</param>
        Task DecreasePositionAsync(string userId, Guid positionId);

        /// <summary>
        /// Delete cart of target user;
        /// </summary>        
        /// <param name="userId">Target userId</param>
        Task DeleteAsync(string userId);

        /// <summary>
        /// Delete target position in users cart. If positions count should become 0, deletes the cart.
        /// </summary>        
        /// <param name="userId">Target UserId</param>
        /// <param name="positionId">Target positionId</param>
        Task DeletePositionAsync(string userId, Guid positionId);

        /// <summary>
        /// Replace user cart to cart from cookies (if exist)
        /// </summary>        
        /// <param name="userId">Target UserId</param>
        /// <param name="cookieCart">Target cookieCart model</param>
        Task ReplaceFromCookieAsync(CartViewModel cookieCart, string userId);
    }
}
