using AutoMapper;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class ComparisonsService
    {
        private readonly IComparisonsRepository _comparisonsRepository;
        private readonly IMapper _mapper;
        private readonly ProductsService _productsService;

        public ComparisonsService(IComparisonsRepository comparisonsRepository, IMapper mapper, ProductsService productsService)
        {
            _comparisonsRepository = comparisonsRepository;
            _mapper = mapper;
            _productsService = productsService;
        }

        /// <summary>
        /// Get all Comparisons for target user by Id
        /// </summary>
        /// <returns>List of ComparisonProductViewModel for target user</returns>
        /// <param name="userId">User Id (GUID)</param>
        public async Task<List<ComparisonProductViewModel>> GetAllAsync(Guid userId)
        {
            return (await _comparisonsRepository.GetAllAsync())
                                                .Where(c => c.UserId == userId)
                                                .Select(_mapper.Map<ComparisonProductViewModel>)
                                                .ToList();
        }

        /// <summary>
        /// Get all Comparisons groups for target user by Id
        /// </summary>
        /// <returns>ILookup object of ComparisonProducts grouping by ProductCategory </returns>
        /// <param name="userId">User Id (GUID)</param>
        public async Task<ILookup<ProductCategoriesViewModel, ComparisonProductViewModel>> GetGroupsAsync(Guid userId)
        {
            return (await GetAllAsync(userId)).ToLookup(c => c.Product.Category);
        }

        /// <summary>
        /// Create a new ComparisonProduct for related user.
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

            var comparison = new ComparisonProduct()
            {
                UserId = userId,
                Product = product
            };

            await _comparisonsRepository.CreateAsync(comparison);
        }

        /// <summary>
        /// Delete target ComparisonProduct by Id
        /// </summary>        
        /// <param name="comparisonId">ComparisonProduct Id (GUID)</param>
        public async Task DeleteAsync(Guid comparisonId)
        {
            await _comparisonsRepository.DeleteAsync(comparisonId);
        }

        /// <summary>
        /// Delete all ComparisonProducts for related user.
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        public async Task DeleteAllAsync(Guid userId)
        {
            await _comparisonsRepository.DeleteAllAsync(userId);
        }

        /// <summary>
        /// Checks if the given product exists in users comparison products
        /// </summary>
        /// <returns>true if product exists; otherwise returns false</returns>
        /// <param name="product">Target Product</param>
        /// <param name="userId">User Id (GUID)</param>
        private async Task<bool> IsProductExistsAsync(Product product, Guid userId)
        {
            return (await GetAllAsync(userId)).Any(c => c.Product.Id == product.Id);
        }
    }
}