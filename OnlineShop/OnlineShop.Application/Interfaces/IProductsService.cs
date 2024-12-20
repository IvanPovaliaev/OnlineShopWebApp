﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShop.Application.Interfaces
{
    public interface IProductsService
    {
        /// <summary>
        /// Get all products from repository
        /// </summary>
        /// <returns>List of all products from repository</returns>
        Task<List<ProductViewModel>> GetAllAsync();

        /// <summary>
        /// Get all products from repository for current category
        /// </summary>        
        /// <returns>List of all products from repository for current category</returns>
        /// <param name="category">Product category</param>
        Task<List<ProductViewModel>> GetAllAsync(ProductCategoriesViewModel category);

        /// <summary>
        /// Get all products from repository that match the search query. The search is performed by name;
        /// </summary>        
        /// <returns>List of all relevant products</returns>
        /// <param name="searchQuery">Search query</param>
        Task<List<ProductViewModel>> GetAllFromSearchAsync(string searchQuery);

        /// <summary>
        /// Get all new products from repository for target quantity
        /// </summary>        
        /// <returns>List of all relevant products</returns>
        /// <param name="quantity">Target quantity of new products</param>
        Task<List<ProductViewModel>> GetAllNew(int quantity);

        /// <summary>
        /// Get product from repository by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        /// <param name="id">Target product id</param>
        Task<Product> GetAsync(Guid id);

        /// <summary>
        /// Get product ViewModel of related product by GUID
        /// </summary>
        /// <returns>ProductViewModel; returns null if product not found</returns>
        /// <param name="id">Target product id</param>
        Task<ProductViewModel> GetViewModelAsync(Guid id);

        /// <summary>
        /// Get EditProduct from repository by GUID
        /// </summary>
        /// <returns>EditProductViewModel; returns null if product not found</returns>
        /// <param name="id">Target product id</param>
        Task<EditProductViewModel> GetEditProductAsync(Guid id);

        /// <summary>
        /// Add product to repository
        /// </summary>
        /// <param name="product">Target product</param>
        Task AddAsync(AddProductViewModel product);

        /// <summary>
        /// Update product with identical id.
        /// </summary>
        /// <param name="product">Target product</param>
        Task UpdateAsync(EditProductViewModel product);

        /// <summary>
        /// Delete product from repository by its ID
        /// </summary>
        /// <param name="id">Target product id</param>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Validates the order edit product model
        /// </summary>        
        /// <returns>true if creation model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="product">Target edit product model</param>
        Task<bool> IsUpdateValidAsync(ModelStateDictionary modelState, EditProductViewModel product);

        /// <summary>
        /// Get the IProductSpecificationsRules implementation according to the target category
        /// </summary>        
        /// <returns>Related IProductSpecificationsRules representation</returns>
        /// <param name="category">ProductCategoriesViewModel</param>
        IProductSpecificationsRules GetSpecificationsRules(ProductCategoriesViewModel category);

        /// <summary>
        /// Get MemoryStream for all products export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with products info</returns>
        Task<MemoryStream> ExportAllToExcelAsync();
    }
}
