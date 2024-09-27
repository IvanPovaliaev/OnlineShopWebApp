using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IFavoritesRepository
    {
        /// <summary>
        /// Get all FavoriteProducts
        /// </summary>
        /// <returns>List of all products</returns>
        List<FavoriteProduct> GetAll();

        /// <summary>
        /// Get a FavoriteProducts by Id
        /// </summary>
        /// <returns>Target FavoriteProducts</returns>
        /// <param name="favoriteProductId">FavoriteProduct Id (guid)</param>
        FavoriteProduct Get(Guid favoriteProductId);

        /// <summary>
        /// Create a new FavoriteProduct
        /// </summary>
        /// <param name="product">Target FavoriteProduct</param>
        void Create(FavoriteProduct product);

        /// <summary>
        /// Delete a FavoriteProduct by Id.
        /// </summary>
        /// <param name="favoriteProductId">FavoriteProduct Id (guid)</param>
        void Delete(Guid favoriteProductId);

        /// <summary>
        /// Delete all FavoriteProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        void DeleteAll(Guid userId);
    }
}
