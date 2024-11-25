using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Helpers.Specifications;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IExcelService _excelService;
        private readonly IEnumerable<IProductSpecificationsRules> _specificationsRules;
        private readonly IMapper _mapper;
        private readonly ImagesProvider _imageProvider;
        private readonly string _productsImagesStoragePath;
        private readonly RedisHashService _redisHashService;
        private readonly string _redisProductsHashKey = "products_list";

        public ProductsService(IProductsRepository productsRepository, IMapper mapper, IExcelService excelService, IEnumerable<IProductSpecificationsRules> specificationsRules, IConfiguration configuration, ImagesProvider imagesProvider, RedisHashService redisCacheService)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _excelService = excelService;
            _specificationsRules = specificationsRules;

            _productsImagesStoragePath = configuration["ImagesStorage:Products"]!;
            _imageProvider = imagesProvider;
            _redisHashService = redisCacheService;
        }

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var cachedProducts = await _redisHashService.GetAllValuesAsync(_redisProductsHashKey);

            if (!cachedProducts.IsNullOrEmpty())
            {
                return cachedProducts!.Select(JsonConvert.DeserializeObject<ProductViewModel>)
                                                   .ToList()!;
            }

            var products = await _productsRepository.GetAllAsync();
            var productViewModels = products.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();

            var productsDictionary = productViewModels.ToDictionary(x => x.Id.ToString(), JsonConvert.SerializeObject);
            await _redisHashService.SetHashFieldsAsync(_redisProductsHashKey, productsDictionary);


            return productViewModels!;
        }

        public async Task<List<ProductViewModel>> GetAllAsync(ProductCategoriesViewModel category)
        {
            var specification = new ProductByCategorySpecification((ProductCategories)category);

            var products = await _productsRepository.GetAllAsync(specification);
            return products.Select(_mapper.Map<ProductViewModel>)
                           .ToList();
        }

        public async Task<List<ProductViewModel>> GetAllFromSearchAsync(string searchQuery)
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

        public async Task<Product> GetAsync(Guid id) => await _productsRepository.GetAsync(id);

        public async Task<ProductViewModel> GetViewModelAsync(Guid id)
        {
            var cachedProduct = await _redisHashService.TryGetHashFieldAsync(_redisProductsHashKey, id.ToString());

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                return JsonConvert.DeserializeObject<ProductViewModel>(cachedProduct)!;
            }

            var productDb = await GetAsync(id);
            return _mapper.Map<ProductViewModel>(productDb);
        }

        public async Task<EditProductViewModel> GetEditProductAsync(Guid id)
        {
            var productDb = await GetAsync(id);
            return _mapper.Map<EditProductViewModel>(productDb);
        }

        public async Task AddAsync(AddProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            var images = await SaveImagesAsync(productDb.Id, product.UploadedImages!);
            productDb.Images = images;

            await _productsRepository.AddAsync(productDb);
            await CacheProduct(productDb);
        }

        public async Task UpdateAsync(EditProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            var images = await SaveImagesAsync(productDb.Id, product.UploadedImages!);
            productDb.Images = images;

            await _productsRepository.UpdateAsync(productDb);

            await CacheProduct(productDb);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _productsRepository.DeleteAsync(id);

            await _redisHashService.RemoveHashFieldAsync(_redisProductsHashKey, id.ToString());
        }

        public async Task<bool> IsUpdateValidAsync(ModelStateDictionary modelState, EditProductViewModel product)
        {
            var repositoryProduct = await GetViewModelAsync(product.Id);

            if (repositoryProduct.Category != product.Category)
            {
                modelState.AddModelError(string.Empty, "Изменена категория продукта.");
            }

            return modelState.IsValid;
        }

        public IProductSpecificationsRules GetSpecificationsRules(ProductCategoriesViewModel category)
        {
            var categoryDb = (ProductCategories)category;
            return _specificationsRules.FirstOrDefault(s => s.Category == categoryDb)!;
        }

        public async Task<MemoryStream> ExportAllToExcelAsync()
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

        /// <summary>
        /// Save images to local storage and return List of related ProductImage
        /// </summary>
        /// <param name="productId">Related product Id</param>
        /// <param name="uploadedImages">Target collection of images</param>
        private async Task<List<ProductImage>> SaveImagesAsync(Guid productId, ICollection<IFormFile> uploadedImages)
        {
            var imagesUrls = await _imageProvider.SaveAllAsync(uploadedImages, _productsImagesStoragePath);

            var images = new List<ProductImage>(imagesUrls.Count);

            foreach (var imageUrl in imagesUrls)
            {
                var image = new ProductImage()
                {
                    Url = imageUrl,
                    ProductId = productId
                };
                images.Add(image);
            }

            return images;
        }

        private async Task CacheProduct(Product product)
        {
            var productVMJson = JsonConvert.SerializeObject(_mapper.Map<ProductViewModel>(product));
            await _redisHashService.SetHashFieldAsync(_redisProductsHashKey, product.Id.ToString(), productVMJson);
        }
    }
}
