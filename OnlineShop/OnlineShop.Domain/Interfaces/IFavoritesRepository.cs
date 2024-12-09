using LinqSpecs;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Domain.Interfaces
{
    public interface IFavoritesRepository
    {
        /// <summary>
        /// Get all FavoriteProducts
        /// </summary>
        /// <returns>List of all products</returns>
        /// <param name="specification">Specification for favorites</param>
        Task<List<FavoriteProduct>> GetAllAsync(Specification<FavoriteProduct>? specification = null);

        /// <summary>
        /// Get a FavoriteProducts by Id
        /// </summary>
        /// <returns>Target FavoriteProducts</returns>
        /// <param name="id">FavoriteProduct Id (guid)</param>
        Task<FavoriteProduct?> GetAsync(Guid id);

        /// <summary>
        /// Create a new FavoriteProduct
        /// </summary>
        /// <param name="product">Target FavoriteProduct</param>
        Task<Guid?> CreateAsync(FavoriteProduct product);

        /// <summary>
        /// Delete a FavoriteProduct by Id.
        /// </summary>
        /// <param name="id">FavoriteProduct Id (guid)</param>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Delete all FavoriteProducts by userId.
        /// </summary>
        /// <param name="userId">User Id</param>
        Task<bool> DeleteAllAsync(string userId);
    }
}
