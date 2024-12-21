using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using OnlineShop.Application.Helpers;
using OnlineShop.Application.Helpers.Specifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Application.Models.Options;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IExcelService _excelService;
        private readonly IEnumerable<IProductSpecificationsRules> _specificationsRules;
        private readonly IMapper _mapper;
        private readonly ImagesProvider _imageProvider;
        private readonly string _productsImagesStoragePath;

        public ProductsService(IProductsRepository productsRepository, IMapper mapper, IExcelService excelService, IEnumerable<IProductSpecificationsRules> specificationsRules, IOptions<ImagesStorage> imagesStorage, ImagesProvider imagesProvider)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _excelService = excelService;
            _specificationsRules = specificationsRules;

            _productsImagesStoragePath = imagesStorage.Value.ProductsPath;
            _imageProvider = imagesProvider;
        }

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var products = await _productsRepository.GetAllAsync();

            return products.Select(_mapper.Map<ProductViewModel>)
                                            .ToList();
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

            var searchSpecification = new ProductsBySearchSpecification(searchQuery);

            var products = await _productsRepository.GetAllAsync(searchSpecification);

            return products.Select(_mapper.Map<ProductViewModel>)
                           .ToList();
        }

        public async Task<List<ProductViewModel>> GetAllNew(int quantity)
        {
            var products = await _productsRepository.GetAllAsync();

            return products.OrderByDescending(p => p.CreationDate)
                           .Take(quantity)
                           .Select(_mapper.Map<ProductViewModel>)
                           .ToList();
        }

        public async Task<Product> GetAsync(Guid id) => await _productsRepository.GetAsync(id);

        public async Task<ProductViewModel> GetViewModelAsync(Guid id)
        {
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
        }

        public async Task UpdateAsync(EditProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            var images = await SaveImagesAsync(productDb.Id, product.UploadedImages!);

            if (images.Count != 0)
            {
                productDb.Images = images;
            }

            await _productsRepository.UpdateAsync(productDb);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _productsRepository.DeleteAsync(id);
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
    }
}
