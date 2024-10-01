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
        void Add(List<Product> products);

        /// <summary>
        /// Add product
        /// </summary>
        /// <param name="product">Target Product</param>
        void Add(Product product);

        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="product">Target Product</param>
        void Update(Product product);

        /// <summary>
        /// Delete product by GUID
        /// </summary>
        /// <param name="id">Product Id</param>
        void Delete(Guid id);
    }
}
