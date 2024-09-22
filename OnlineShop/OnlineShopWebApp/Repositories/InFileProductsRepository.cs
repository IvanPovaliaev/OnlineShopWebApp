using Newtonsoft.Json;
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
        private FileService _fileService;
        private List<Product> _products;

        public InFileProductsRepository(FileService fileService)
        {
            _fileService = fileService;
            Upload();
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
            SaveChanges();
        }

        /// <summary>
        /// Save changes to storage
        /// </summary>
        private void SaveChanges()
        {
            var jsonData = JsonConvert.SerializeObject(_products, Formatting.Indented);
            _fileService.Save(FilePath, jsonData);
        }

        /// <summary>
        /// Upload products
        /// </summary>   
        private void Upload()
        {
            if (!_fileService.Exists(FilePath) || string.IsNullOrEmpty(_fileService.GetContent(FilePath)))
            {
                _products = [];
                return;
            }

            var productsJson = _fileService.GetContent(FilePath);

            _products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
        }
    }
}
