using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Redis
{
    public class RedisProductsService : IProductsService
    {
        private readonly IProductsService _decoretedProductsService;
        private readonly string _redisProductsKey;
        private readonly RedisService _redisService;

        public RedisProductsService(IProductsService productsService, IOptions<RedisSettings> redisSettings, RedisService redisCacheService)
        {
            _decoretedProductsService = productsService;
            _redisProductsKey = redisSettings.Value.ProductsKey!;
            _redisService = redisCacheService;
        }

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var cachedProducts = await _redisService.TryGetAsync(_redisProductsKey);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                return JsonConvert.DeserializeObject<List<ProductViewModel>>(cachedProducts)!;
            }

            var products = await _decoretedProductsService.GetAllAsync();

            await _redisService.SetAsync(_redisProductsKey, JsonConvert.SerializeObject(products));

            return products;
        }

        public async Task<List<ProductViewModel>> GetAllAsync(ProductCategoriesViewModel category)
        {
            return await _decoretedProductsService.GetAllAsync(category);
        }

        public async Task<List<ProductViewModel>> GetAllFromSearchAsync(string searchQuery)
        {
            return await _decoretedProductsService.GetAllFromSearchAsync(searchQuery);
        }

        public async Task<List<ProductViewModel>> GetAllNew(int quantity)
        {
            return await _decoretedProductsService.GetAllNew(quantity);
        }

        public async Task<Product> GetAsync(Guid id)
        {
            return await _decoretedProductsService.GetAsync(id);
        }

        public async Task<ProductViewModel> GetViewModelAsync(Guid id)
        {
            var cachedProduct = await _redisService.TryGetAsync(id.ToString());

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                return JsonConvert.DeserializeObject<ProductViewModel>(cachedProduct)!;
            }

            var product = await _decoretedProductsService.GetViewModelAsync(id);

            await _redisService.SetAsync(id.ToString(), JsonConvert.SerializeObject(product));

            return product;
        }

        public async Task<EditProductViewModel> GetEditProductAsync(Guid id)
        {
            return await _decoretedProductsService.GetEditProductAsync(id);
        }

        public async Task AddAsync(AddProductViewModel product)
        {
            await _decoretedProductsService.AddAsync(product);
            await ClearProductCacheAsync();
        }

        public async Task UpdateAsync(EditProductViewModel product)
        {
            await _decoretedProductsService.UpdateAsync(product);
            await ClearProductCacheAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var isSuccess = await _decoretedProductsService.DeleteAsync(id);
            if (!isSuccess)
            {
                return false;
            }
            await ClearProductCacheAsync();
            await _redisService.RemoveAsync(id.ToString());
            return true;
        }

        public async Task<bool> IsUpdateValidAsync(ModelStateDictionary modelState, EditProductViewModel product)
        {
            return await _decoretedProductsService.IsUpdateValidAsync(modelState, product);
        }

        public IProductSpecificationsRules GetSpecificationsRules(ProductCategoriesViewModel category)
        {
            return _decoretedProductsService.GetSpecificationsRules(category);
        }

        public async Task<MemoryStream> ExportAllToExcelAsync()
        {
            return await _decoretedProductsService.ExportAllToExcelAsync();
        }

        /// <summary>
        /// Clear cache with all products
        /// </summary>
        private async Task ClearProductCacheAsync()
        {
            await _redisService.RemoveAsync(_redisProductsKey);
        }
    }
}
