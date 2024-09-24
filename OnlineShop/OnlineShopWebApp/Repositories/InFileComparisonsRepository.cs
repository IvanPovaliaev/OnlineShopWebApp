using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

namespace OnlineShopWebApp.Repositories
{
    public class InFileComparisonsRepository : IComparisonRepository
    {
        public const string FilePath = @".\Data\ComparisonProducts.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<ComparisonProduct> _comparisons;

        public InFileComparisonsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _comparisons = _jsonRepositoryService.Upload<ComparisonProduct>(FilePath);
        }

        public void Create(ComparisonProduct product)
        {
            _comparisons.Add(product);
            _jsonRepositoryService.SaveChanges(FilePath, _comparisons);
        }
    }
}
