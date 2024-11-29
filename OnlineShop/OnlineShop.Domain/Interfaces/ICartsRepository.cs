using OnlineShop.Domain.Models;
using System.Threading.Tasks;

namespace OnlineShop.Domain.Interfaces
{
    public interface ICartsRepository
    {
        /// <summary>
        /// Get cart by userId (guid)
        /// </summary>        
        /// <returns>Cart for related user</returns>
        /// <param name="userId">GUID user id</param>
        Task<Cart> GetAsync(string userId);

        /// <summary>
        /// Create a new cart
        /// </summary> 
        /// <param name="cart">Target cart</param>
        Task CreateAsync(Cart cart);

        /// <summary>
        /// Update cart with identical id. If cart is not in the repository - does nothing.
        /// </summary>
        /// <param name="cart">Target cart</param>
        Task UpdateAsync(Cart cart);

        /// <summary>
        /// Delete cart by userId. If cart is not in the repository - does nothing.
        /// </summary>
        /// <param name="userId">GUID user id</param>
        Task DeleteAsync(string userId);
    }
}
