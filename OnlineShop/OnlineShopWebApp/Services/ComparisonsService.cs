using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class ComparisonsService
    {
        private readonly IComparisonRepository _comparisonsRepository;
        private readonly ProductsService _productsService;

        public ComparisonsService(IComparisonRepository comparisonsRepository, ProductsService productsService)
        {
            _comparisonsRepository = comparisonsRepository;
            _productsService = productsService;
        }

        /// <summary>
        /// Get all Comparisons for target user by Id
        /// </summary>        
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
        /// Create a new ComparisonProduct for related user.
        /// </summary>        
        /// <param name="productId">Product Id (GUID)</param>
        /// <param name="userId">User Id (GUID)</param>
        public void Create(Guid productId, Guid userId)
        {
            var product = _productsService.Get(productId);
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
        /// <param name="comparisonId">User Id (GUID)</param>
        public void DeleteAll(Guid userId)
        {
            var userComparisons = _comparisonsRepository
                .GetAll()
                .Where(c => c.UserId == userId)
                .ToArray();

            foreach (var comparison in userComparisons)
            {
                _comparisonsRepository.Delete(comparison.Id);
            }
        }
    }
}
