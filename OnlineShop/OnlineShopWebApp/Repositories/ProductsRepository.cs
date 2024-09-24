﻿using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class ProductsRepository
    {
        public const string FilePath = @".\Data\Products.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Product> _products;

        public ProductsRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _products = _jsonRepositoryService.Upload<Product>(FilePath);
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        public List<Product> GetAll() => _products;

        /// <summary>
        /// Get product by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        public Product Get(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product;
        }

        /// <summary>
        /// Add list of products
        /// </summary>
        /// <param name="products">Products list</param>
        public void Add(List<Product> products)
        {
            _products.AddRange(products);
            _jsonRepositoryService.SaveChanges(FilePath, _products);
        }
    }
}
