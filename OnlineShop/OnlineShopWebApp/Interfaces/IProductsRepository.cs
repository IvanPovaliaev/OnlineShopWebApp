using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IProductsRepository
    {
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        List<Product> GetAll();

        /// <summary>
        /// Get product by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        Product Get(Guid id);

        /// <summary>
        /// Add list of products
        /// </summary>
        /// <param name="products">Products list</param>
        public void Add(List<Product> products);
    }
}
