﻿using AutoMapper;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class FavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IMapper _mapper;
        private readonly ProductsService _productsService;

        public FavoritesService(IFavoritesRepository favoritesRepository, IMapper mapper, ProductsService productsService)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        /// <summary>
        /// Get all Favorites for target user by Id
        /// </summary>
        /// <returns>List of FavoriteProductViewModel for target user</returns>
        /// <param name="userId">User Id (GUID)</param>
        public async Task<List<FavoriteProductViewModel>> GetAllAsync(Guid userId)
        {
            var favorites = await _favoritesRepository.GetAllAsync();

            return favorites.Where(c => c.UserId == userId)
                            .Select(_mapper.Map<FavoriteProductViewModel>)
                            .ToList();
        }

        /// <summary>
        /// Create a new FavoriteProduct for related user.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public async Task CreateAsync(Guid productId, Guid userId)
        {
            var product = await _productsService.GetAsync(productId);

            if (await IsProductExistsAsync(product, userId))
            {
                return;
            }

            var favorite = new FavoriteProduct()
            {
                UserId = userId,
                Product = product
            };

            await _favoritesRepository.CreateAsync(favorite);
        }

        /// <summary>
        /// Delete target FavoriteProduct by Id
        /// </summary>        
        /// <param name="id">FavoriteProduct Id (GUID)</param>
        public async Task DeleteAsync(Guid id)
        {
            await _favoritesRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Delete all FavoriteProducts for related user.
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        public async Task DeleteAllAsync(Guid userId)
        {
            await _favoritesRepository.DeleteAllAsync(userId);
        }

        /// <summary>
        /// Checks if the given product exists in users favorites products
        /// </summary>
        /// <returns>true if product exists; otherwise returns false</returns>
        /// <param name="product">Target Product</param>
        /// <param name="userId">User Id (GUID)</param>
        private async Task<bool> IsProductExistsAsync(Product product, Guid userId)
        {
            var result = await GetAllAsync(userId);

            return result.Any(c => c.Product.Id == product.Id);
        }
    }
}