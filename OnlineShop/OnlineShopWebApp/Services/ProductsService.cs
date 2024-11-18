using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class ProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IExcelService _excelService;
        private readonly IEnumerable<IProductSpecificationsRules> _specificationsRules;
        private readonly IMapper _mapper;
        private readonly ImagesProvider _imageProvider;
        private readonly string _productsImagesStoragePath;

        public ProductsService(IProductsRepository productsRepository, IMapper mapper, IExcelService excelService, IEnumerable<IProductSpecificationsRules> specificationsRules, IConfiguration configuration, ImagesProvider imagesProvider)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _excelService = excelService;
            _specificationsRules = specificationsRules;

            _productsImagesStoragePath = configuration["ImagesStorage:Products"]!;
            _imageProvider = imagesProvider;
        }

        /// <summary>
        /// Get all products from repository
        /// </summary>
        /// <returns>List of all products from repository</returns>
        public virtual async Task<List<ProductViewModel>> GetAllAsync()
        {
            var products = await _productsRepository.GetAllAsync();
            return products.Select(_mapper.Map<ProductViewModel>)
                           .ToList();
        }

        /// <summary>
        /// Get all products from repository for current category
        /// </summary>        
        /// <returns>List of all products from repository for current category</returns>
        /// <param name="category">Product category</param>
        public virtual async Task<List<ProductViewModel>> GetAllAsync(ProductCategoriesViewModel category)
        {
            var products = await GetAllAsync();
            return products.Where(p => p.Category == category)
                           .ToList();
        }

        /// <summary>
        /// Get all products from repository that match the search query. The search is performed by name and by article (if possible);
        /// </summary>        
        /// <returns>List of all relevant products</returns>
        /// <param name="searchQuery">Search query</param>
        public virtual async Task<List<ProductViewModel>> GetAllFromSearchAsync(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return [];
            }

            var isProductInfoContainsString = IsNameContainsString;

            var isNumber = long.TryParse(searchQuery, out _);

            if (isNumber)
            {
                isProductInfoContainsString = (ProductViewModel product, string targetString) =>
                {
                    var result = IsNameContainsString(product, targetString) || IsArticleContainsNumber(product, targetString);
                    return result;
                };
            }

            var products = await GetAllAsync();

            return products.Where(p => isProductInfoContainsString(p, searchQuery))
                           .ToList(); ;
        }

        /// <summary>
        /// Get product from repository by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        public virtual async Task<Product> GetAsync(Guid id) => await _productsRepository.GetAsync(id);

        /// <summary>
        /// Get product ViewModel of related product by GUID
        /// </summary>
        /// <returns>ProductViewModel; returns null if product not found</returns>
        public virtual async Task<ProductViewModel> GetViewModelAsync(Guid id)
        {
            var productDb = await GetAsync(id);
            return _mapper.Map<ProductViewModel>(productDb);
        }

        /// <summary>
        /// Get EditProduct from repository by GUID
        /// </summary>
        /// <returns>EditProductViewModel; returns null if product not found</returns>
        public virtual async Task<EditProductViewModel> GetEditProductAsync(Guid id)
        {
            var productDb = await GetAsync(id);
            return _mapper.Map<EditProductViewModel>(productDb);
        }

        /// <summary>
        /// Add product to repository
        /// </summary>
        /// <param name="product">Target product</param>
        public virtual async Task AddAsync(AddProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            var imageUrl = _imageProvider.Save(product.UploadedImage, _productsImagesStoragePath);
            productDb.ImageUrl = imageUrl;

            await _productsRepository.AddAsync(productDb);
        }

        /// <summary>
        /// Update product with identical id.
        /// </summary>
        /// <param name="product">Target product</param>
        public virtual async Task UpdateAsync(EditProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            var imageUrl = _imageProvider.Save(product.UploadedImage, _productsImagesStoragePath);
            productDb.ImageUrl = imageUrl;

            await _productsRepository.UpdateAsync(productDb);
        }

        /// <summary>
        /// Delete product from repository by GUID
        /// </summary>
        public virtual async Task DeleteAsync(Guid id) => await _productsRepository.DeleteAsync(id);

        /// <summary>
        /// Validates the product update model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="product">Target EditProductViewModel</param>
        public virtual async Task<bool> IsUpdateValidAsync(ModelStateDictionary modelState, EditProductViewModel product)
        {
            var repositoryProduct = await GetViewModelAsync(product.Id);

            if (repositoryProduct.Category != product.Category)
            {
                modelState.AddModelError(string.Empty, "Изменена категория продукта.");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Get the IProductSpecificationsRules implementation according to the target category
        /// </summary>        
        /// <returns>Related IProductSpecificationsRules representation</returns>
        /// <param name="category">ProductCategoriesViewModel</param>
        public virtual IProductSpecificationsRules GetSpecificationsRules(ProductCategoriesViewModel category)
        {
            var categoryDb = (ProductCategories)category;
            return _specificationsRules.FirstOrDefault(s => s.Category == categoryDb)!;
        }

        /// <summary>
        /// Get MemoryStream for all products export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with products info</returns>
        public virtual async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var products = await GetAllAsync();
            return _excelService.ExportProducts(products);
        }

        /// <summary>
        /// Check is Product Name contains a string
        /// </summary>
        /// <param name="product">Target product</param>
        /// <param name="targetString">Target string</param>
        private bool IsNameContainsString(ProductViewModel product, string targetString)
        {
            return product.Name.Contains(targetString);
        }

        /// <summary>
        /// Check is Article contains a number string
        /// </summary>
        /// <param name="product">Target product</param>
        /// <param name="targetNumber">Target string number</param>
        private bool IsArticleContainsNumber(ProductViewModel product, string targetNumber)
        {
            var result = product.Article.ToString()
                                        .Contains(targetNumber);
            return result;
        }
    }
}
