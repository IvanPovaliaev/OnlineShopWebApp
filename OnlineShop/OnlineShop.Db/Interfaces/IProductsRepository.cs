using LinqSpecs;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IProductsRepository
    {
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        /// <param name="specification">Specification for products</param>
        Task<List<Product>> GetAllAsync(Specification<Product>? specification = null);

        /// <summary>
        /// Get product by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        Task<Product> GetAsync(Guid id);

        /// <summary>
        /// Add list of products
        /// </summary>
        /// <param name="products">Products list</param>
        Task AddRangeAsync(List<Product> products);

        /// <summary>
        /// Add product
        /// </summary>
        /// <param name="product">Target Product</param>
        Task AddAsync(Product product);

        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="product">Target Product</param>
        Task UpdateAsync(Product product);

        /// <summary>
        /// Delete product by GUID
        /// </summary>
        /// <param name="id">Product Id</param>
        Task DeleteAsync(Guid id);
    }
}
