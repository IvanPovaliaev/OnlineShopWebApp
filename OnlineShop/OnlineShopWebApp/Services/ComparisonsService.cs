using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class ComparisonsService
    {
        private readonly IComparisonsRepository _comparisonsRepository;
        private readonly ProductsService _productsService;

        public ComparisonsService(IComparisonsRepository comparisonsRepository, ProductsService productsService)
        {
            _comparisonsRepository = comparisonsRepository;
            _productsService = productsService;
        }

        /// <summary>
        /// Get all Comparisons for target user by Id
        /// </summary>
        /// <returns>List of ComparisonProduct for target user</returns>
        /// <param name="userId">User Id (GUID)</param>
        public List<ComparisonProduct> GetAll(Guid userId)
        {
            var comparisons = _comparisonsRepository
                .GetAll()
                .Where(c => c.UserId == userId)
                .ToList();

            return comparisons;
        }

        /// <summary>
        /// Get all Comparisons groups for target user by Id
        /// </summary>
        /// <returns>ILookup object of ComparisonProducts grouping by ProductCategory </returns>
        /// <param name="userId">User Id (GUID)</param>
        public ILookup<ProductCategoriesViewModel, ComparisonProduct> GetGroups(Guid userId)
        {
            var comparisonsGroups = GetAll(userId)
                .ToLookup(c => c.Product.Category);

            return comparisonsGroups;
        }

        /// <summary>
        /// Create a new ComparisonProduct for related user.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public void Create(Guid productId, Guid userId)
        {
            var product = _productsService.GetViewModel(productId);
            if (IsProductExists(product, userId))
            {
                return;
            }
            var comparison = new ComparisonProduct(userId, product);
            _comparisonsRepository.Create(comparison);
        }

        /// <summary>
        /// Delete target ComparisonProduct by Id
        /// </summary>        
        /// <param name="comparisonId">ComparisonProduct Id (GUID)</param>
        public void Delete(Guid comparisonId)
        {
            _comparisonsRepository.Delete(comparisonId);
        }

        /// <summary>
        /// Delete all ComparisonProducts for related user.
        /// </summary>        
        /// <param name="userId">User Id (GUID)</param>
        public void DeleteAll(Guid userId)
        {
            _comparisonsRepository.DeleteAll(userId);
        }

        /// <summary>
        /// Delete all ComparisonProducts related to product Id.
        /// </summary>
        /// <param name="productId">Target product Id (guid)</param>
        public void DeleteAllByProductId(Guid productId)
        {
            _comparisonsRepository.DeleteAllByProductId(productId);
        }

        /// <summary>
        /// Checks if the given product exists in users comparison products
        /// </summary>
        /// <returns>true if product exists; otherwise returns false</returns>
        /// <param name="product">Target Product</param>
        /// <param name="userId">User Id (GUID)</param>
        private bool IsProductExists(ProductViewModel product, Guid userId)
        {
            var result = GetAll(userId)
                .Any(c => c.Product.Id == product.Id);

            return result;
        }
    }
}