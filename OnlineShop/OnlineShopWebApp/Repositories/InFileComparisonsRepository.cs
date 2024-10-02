using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileComparisonsRepository : IComparisonsRepository
    {
        public const string FilePath = @".\Data\ComparisonProducts.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<ComparisonProduct> _comparisons;

        public InFileComparisonsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _comparisons = _jsonRepositoryService.Upload<ComparisonProduct>(FilePath);
        }

        public List<ComparisonProduct> GetAll() => _comparisons;

        public ComparisonProduct Get(Guid comparisonId)
        {
            var comparison = _comparisons.FirstOrDefault(c => c.Id == comparisonId);
            return comparison;
        }

        public void Create(ComparisonProduct product)
        {
            _comparisons.Add(product);
            _jsonRepositoryService.SaveChanges(FilePath, _comparisons);
        }

        public void Delete(Guid comparisonId)
        {
            var comparison = Get(comparisonId);
            _comparisons.Remove(comparison);
            _jsonRepositoryService.SaveChanges(FilePath, _comparisons);
        }

        public void DeleteAll(Guid userId)
        {
            _comparisons.RemoveAll(c => c.UserId == userId);
            _jsonRepositoryService.SaveChanges(FilePath, _comparisons);
        }

        public void DeleteAllByProductId(Guid productId)
        {
            _comparisons.RemoveAll(c => c.Product.Id == productId);
            _jsonRepositoryService.SaveChanges(FilePath, _comparisons);
        }
    }
}
