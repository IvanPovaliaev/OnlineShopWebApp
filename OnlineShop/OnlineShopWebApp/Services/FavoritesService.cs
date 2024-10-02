using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class FavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly ProductsService _productsService;

        public FavoritesService(IFavoritesRepository favoritesRepository, ProductsService productsService, ProductsEventService productsEventService)
        {
            _favoritesRepository = favoritesRepository;
            _productsService = productsService;
            productsEventService.ProductDeleted += DeleteAllByProductId;
        }

        /// <summary>
        /// Get all Favorites for target user by Id
        /// </summary>
        /// <returns>List of FavoriteProducts for target user</returns>
        /// <param name="userId">User Id (GUID)</param>
        public List<FavoriteProduct> GetAll(Guid userId)
        {
            var favorites = _favoritesRepository
                .GetAll()
                .Where(c => c.UserId == userId)
                .ToList();

            return favorites;
        }

        /// <summary>
        /// Create a new FavoriteProduct for related user.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public void Create(Guid productId, Guid userId)
        {
            var product = _productsService.Get(productId);

            if (IsProductExists(product, userId))
            {
                return;
            }

            var favorite = new FavoriteProduct(userId, product);
            _favoritesRepository.Create(favorite);
        }

        /// <summary>
        /// Delete target FavoriteProduct by Id
        /// </summary>        
        /// <param name="favoriteProductId">FavoriteProduct Id (GUID)</param>
        public void Delete(Guid favoriteProductId)
        {
            _favoritesRepository.Delete(favoriteProductId);
        }

        /// <summary>
        /// Delete all FavoriteProducts for related user.
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        public void DeleteAll(Guid userId)
        {
            _favoritesRepository.DeleteAll(userId);
        }

        /// <summary>
        /// Delete all FavoriteProducts related to product Id.
        /// </summary>
        /// <param name="productId">Target product Id (guid)</param>
        private void DeleteAllByProductId(Guid productId)
        {
            _favoritesRepository.DeleteAllByProductId(productId);
        }

        /// <summary>
        /// Checks if the given product exists in users favorites products
        /// </summary>
        /// <returns>true if product exists; otherwise returns false</returns>
        /// <param name="product">Target Product</param>
        /// <param name="userId">User Id (GUID)</param>
        private bool IsProductExists(Product product, Guid userId)
        {
            var result = GetAll(userId)
                .Any(c => c.Product.Id == product.Id);

            return result;
        }
    }
}