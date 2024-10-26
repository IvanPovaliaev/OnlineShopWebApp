using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Interfaces
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
        /// <param name="id">FavoriteProduct Id (guid)</param>
        FavoriteProduct Get(Guid id);

        /// <summary>
        /// Create a new FavoriteProduct
        /// </summary>
        /// <param name="product">Target FavoriteProduct</param>
        void Create(FavoriteProduct product);

        /// <summary>
        /// Delete a FavoriteProduct by Id.
        /// </summary>
        /// <param name="id">FavoriteProduct Id (guid)</param>
        void Delete(Guid id);

        /// <summary>
        /// Delete all FavoriteProducts by userId.
        /// </summary>
        /// <param name="userId">User Id (guid)</param>
        void DeleteAll(Guid userId);

        /// <summary>
        /// Delete all FavoriteProducts related to product Id.
        /// </summary>
        /// <param name="productId">Target product Id (guid)</param>
        void DeleteAllByProductId(Guid productId);
    }
}
