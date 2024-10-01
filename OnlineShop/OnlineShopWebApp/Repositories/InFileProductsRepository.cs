using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileProductsRepository : IProductsRepository
    {
        public const string FilePath = @".\Data\Products.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Product> _products;

        public InFileProductsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _products = _jsonRepositoryService.Upload<Product>(FilePath);
        }

        public List<Product> GetAll() => _products;

        public Product Get(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product;
        }

        public void Add(List<Product> products)
        {
            _products.AddRange(products);
            _jsonRepositoryService.SaveChanges(FilePath, _products);
        }

        public void Delete(Guid id)
        {
            var product = Get(id);
            _products.Remove(product);
            _jsonRepositoryService.SaveChanges(FilePath, _products);
        }
    }
}
