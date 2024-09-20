using Newtonsoft.Json;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class ProductsRepository
    {
        public const string FilePath = @".\Data\Products.json";
        private List<Product> _products;

        public ProductsRepository()
        {
            Upload();
        }

        public List<Product> GetAll() => _products;

        public Product Get(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product;
        }

        public void Add(IEnumerable<Product> products)
        {
            _products.AddRange(products);
            SaveChanges();
        }

        public void SaveChanges()
        {
            var jsonData = JsonConvert.SerializeObject(_products, Formatting.Indented);
            FileService.Save(FilePath, jsonData);
        }

        private void Upload()
        {
            if (!FileService.Exists(FilePath) || string.IsNullOrEmpty(FileService.GetContent(FilePath)))
            {
                _products = [];
                return;
            }

            var productsJson = FileService.GetContent(FilePath);

            _products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
        }
    }
}
