using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IFavoritesRepository
    {
        /// <summary>
        /// Get all FavoriteProducts
        /// </summary>
        /// <returns>List of all products</returns>
        Task<List<FavoriteProduct>> GetAllAsync();

        /// <summary>
        /// Get a FavoriteProducts by Id
        /// </summary>
        /// <returns>Target FavoriteProducts</returns>
        /// <param name="id">FavoriteProduct Id (guid)</param>
        Task<FavoriteProduct> GetAsync(Guid id);

        /// <summary>
        /// Create a new FavoriteProduct
        /// </summary>
        /// <param name="product">Target FavoriteProduct</param>
        Task CreateAsync(FavoriteProduct product);

        /// <summary>
        /// Delete a FavoriteProduct by Id.
        /// </summary>
        /// <param name="id">FavoriteProduct Id (guid)</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Delete all FavoriteProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        Task DeleteAllAsync(Guid userId);
    }
}
