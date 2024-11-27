using AutoMapper;
using OnlineShop.Application.Helpers.Specifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IMapper _mapper;
        private readonly IProductsService _productsService;

        public FavoritesService(IFavoritesRepository favoritesRepository, IMapper mapper, IProductsService productsService)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        public async Task<List<FavoriteProductViewModel>> GetAllAsync(string userId)
        {
            var specification = new FavoritesByUserIdSpecification(userId);

            var favorites = await _favoritesRepository.GetAllAsync(specification);
            return favorites.Select(_mapper.Map<FavoriteProductViewModel>)
                            .ToList();
        }

        public async Task CreateAsync(Guid productId, string userId)
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

        public async Task DeleteAsync(Guid id)
        {
            await _favoritesRepository.DeleteAsync(id);
        }

        public async Task DeleteAllAsync(string userId)
        {
            await _favoritesRepository.DeleteAllAsync(userId);
        }

        /// <summary>
        /// Checks if the given product exists in users favorites products
        /// </summary>
        /// <returns>true if product exists; otherwise returns false</returns>
        /// <param name="product">Target Product</param>
        /// <param name="userId">User Id (GUID)</param>
        private async Task<bool> IsProductExistsAsync(Product product, string userId)
        {
            var result = await GetAllAsync(userId);

            return result.Any(c => c.Product.Id == product.Id);
        }
    }
}