using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
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

        public ProductsService(IProductsRepository productsRepository, IMapper mapper, IExcelService excelService, IEnumerable<IProductSpecificationsRules> specificationsRules)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _excelService = excelService;
            _specificationsRules = specificationsRules;
            InitializeProducts();
        }

        /// <summary>
        /// Get all products from repository
        /// </summary>
        /// <returns>List of all products from repository</returns>
        public async Task<List<ProductViewModel>> GetAllAsync()
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
        public async Task<List<ProductViewModel>> GetAllAsync(ProductCategoriesViewModel category)
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

        /// <summary>
        /// Get product from repository by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        public async Task<Product> GetAsync(Guid id) => await _productsRepository.GetAsync(id);

        /// <summary>
        /// Get product ViewModel of related product by GUID
        /// </summary>
        /// <returns>ProductViewModel; returns null if product not found</returns>
        public async Task<ProductViewModel> GetViewModelAsync(Guid id)
        {
            var productDb = await GetAsync(id);
            return _mapper.Map<ProductViewModel>(productDb);
        }

        /// <summary>
        /// Get EditProduct from repository by GUID
        /// </summary>
        /// <returns>EditProductViewModel; returns null if product not found</returns>
        public async Task<EditProductViewModel> GetEditProductAsync(Guid id)
        {
            var productDb = await GetAsync(id);
            return _mapper.Map<EditProductViewModel>(productDb);
        }

        /// <summary>
        /// Add product to repository
        /// </summary>
        /// <param name="product">Target product</param>
        public async Task AddAsync(AddProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            await _productsRepository.AddAsync(productDb);
        }

        /// <summary>
        /// Update product with identical id.
        /// </summary>
        /// <param name="product">Target product</param>
        public async Task UpdateAsync(EditProductViewModel product)
        {
            var productDb = _mapper.Map<Product>(product);
            await _productsRepository.UpdateAsync(productDb);
        }

        /// <summary>
        /// Delete product from repository by GUID
        /// </summary>
        public async Task DeleteAsync(Guid id) => await _productsRepository.DeleteAsync(id);

        /// <summary>
        /// Validates the product update model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="product">Target EditProductViewModel</param>
        public async Task<bool> IsUpdateValidAsync(ModelStateDictionary modelState, EditProductViewModel product)
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
        public IProductSpecificationsRules GetSpecificationsRules(ProductCategoriesViewModel category)
        {
            var categoryDb = (ProductCategories)category;
            return _specificationsRules.FirstOrDefault(s => s.Category == categoryDb)!;
        }

        /// <summary>
        /// Get MemoryStream for all products export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with products info</returns>
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
        /// Initializes initial products if repository is empty;
        /// </summary>
        private void InitializeProducts()
        {
            //var products = GetAllAsync();
            //if (products.Count != 0)
            //{
            //    return;
            //}

            //var ssdImageUrl = "/img/products/SSD-1Tb-Kingston-NV2.webp";
            //var ssd = new ProductViewModel("SSD 1Tb Kingston NV2 (SNV2S/1000G)", 7050, "Test Description for SSD", ProductCategoriesViewModel.SSD, ssdImageUrl);
            //ssd.Specifications["Manufacturer"] = "Kingston";
            //ssd.Specifications["ManufacturerCode"] = "SNV2S/1000G";
            //ssd.Specifications["FormFactor"] = "M.2";
            //ssd.Specifications["Capacity"] = "1000 Гб";

            //var hddImageUrl = "/img/products/2Tb-SATA-III-Seagate-Barracuda.webp";
            //var hdd = new ProductViewModel("2Tb SATA-III Seagate Barracuda (ST2000DM008)", 7030, "Test Description for HDD", ProductCategoriesViewModel.HDD, hddImageUrl);
            //hdd.Specifications["Manufacturer"] = "Seagate";
            //hdd.Specifications["ManufacturerCode"] = "ST2000DM008";
            //hdd.Specifications["FormFactor"] = "3.5\"";
            //hdd.Specifications["Interface"] = "SATA-III";
            //hdd.Specifications["Capacity"] = "2000 Гб";

            //var firstRamImageUrl = "/img/products/32Gb-DDR5-6000MHz-Team-T-Create-Expert-_2x16Gb-KIT.webp";
            //var firstRam = new ProductViewModel("32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategoriesViewModel.RAM, firstRamImageUrl);
            //firstRam.Specifications["Manufacturer"] = "Team";
            //firstRam.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            //firstRam.Specifications["FormFactor"] = "DIMM";
            //firstRam.Specifications["MemoryType"] = "DDR5";
            //firstRam.Specifications["MemorySize"] = "32 Гб";
            //firstRam.Specifications["ModulesCount"] = "2";
            //firstRam.Specifications["ClockSpeed"] = "6000 МГц";

            //var secondRamImageUrl = "/img/products/32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT).webp";
            //var secondRam = new ProductViewModel("32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT)", 14160, "Test Description for RAM", ProductCategoriesViewModel.RAM, secondRamImageUrl);
            //secondRam.Specifications["Manufacturer"] = "Kingston";
            //secondRam.Specifications["ManufacturerCode"] = "KF560C40BBK2-32";
            //secondRam.Specifications["FormFactor"] = "DIMM";
            //secondRam.Specifications["MemoryType"] = "DDR5";
            //secondRam.Specifications["MemorySize"] = "32 Гб";
            //secondRam.Specifications["ModulesCount"] = "2";
            //secondRam.Specifications["ClockSpeed"] = "6000 МГц";

            //var thirdRamImageUrl = "/img/products/16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT).webp";
            //var thirdRam = new ProductViewModel("16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT)", 3970, "Test Description for RAM", ProductCategoriesViewModel.RAM, thirdRamImageUrl);
            //thirdRam.Specifications["Manufacturer"] = "Netac";
            //thirdRam.Specifications["ManufacturerCode"] = "NTSWD4P32DP-16W";
            //thirdRam.Specifications["FormFactor"] = "DIMM";
            //thirdRam.Specifications["MemoryType"] = "DDR4";
            //thirdRam.Specifications["MemorySize"] = "16 Гб";
            //thirdRam.Specifications["ModulesCount"] = "2";
            //thirdRam.Specifications["ClockSpeed"] = "3200 МГц";

            //var fourthRamImageUrl = "/img/products/32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT).webp";
            //var fourthRam = new ProductViewModel("32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT)", 8450, "Test Description for RAM", ProductCategoriesViewModel.RAM, fourthRamImageUrl);
            //fourthRam.Specifications["Manufacturer"] = "Patriot MemoryPatriot Memory";
            //fourthRam.Specifications["ManufacturerCode"] = "PVSR432G360C0K";
            //fourthRam.Specifications["FormFactor"] = "DIMM";
            //fourthRam.Specifications["MemoryType"] = "DDR4";
            //fourthRam.Specifications["MemorySize"] = "32 Гб";
            //fourthRam.Specifications["ModulesCount"] = "2";
            //fourthRam.Specifications["ClockSpeed"] = "3600 МГц";

            //var fifthRamImageUrl = "/img/products/64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT).webp";
            //var fifthRam = new ProductViewModel("64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT)", 20790, "Test Description for RAM", ProductCategoriesViewModel.RAM, fifthRamImageUrl);
            //fifthRam.Specifications["Manufacturer"] = "ADATA";
            //fifthRam.Specifications["ManufacturerCode"] = "AX5U5600C3632G-DCLABK";
            //fifthRam.Specifications["FormFactor"] = "DIMM";
            //fifthRam.Specifications["MemoryType"] = "DDR5";
            //fifthRam.Specifications["MemorySize"] = "64 Гб";
            //fifthRam.Specifications["ModulesCount"] = "2";
            //fifthRam.Specifications["ClockSpeed"] = "5600 МГц";

            //var cpuImageUrl = "/img/products/Intel-Core-i5-12400F-OEM.webp";
            //var cpu = new ProductViewModel("Intel Core i5 - 12400F OEM", 15870, "Test Description for CPU", ProductCategoriesViewModel.Processors, cpuImageUrl);
            //cpu.Specifications["Manufacturer"] = "Intel";
            //cpu.Specifications["ManufacturerCode"] = "CM8071504555318/CM8071504650609";
            //cpu.Specifications["Model"] = "Core i5 12400F";
            //cpu.Specifications["Socket"] = "LGA 1700";
            //cpu.Specifications["Architecture"] = "Alder Lake";
            //cpu.Specifications["CoresCount"] = "6";
            //cpu.Specifications["ThreadsCount"] = "12";
            //cpu.Specifications["ClockSpeed"] = "2500 МГц";

            //var powerSupply = new ProductViewModel("750W Be Quiet System Power 10", 8250, "Test Description for PowerSupply", ProductCategoriesViewModel.PowerSupplies);
            //powerSupply.Specifications["Manufacturer"] = "Be Quiet";
            //powerSupply.Specifications["ManufacturerCode"] = "BN329";
            //powerSupply.Specifications["Power"] = "750 Вт";
            //powerSupply.Specifications["PFC"] = "активный";
            //powerSupply.Specifications["FanSize"] = "120 мм";

            //products =
            //[
            //    ssd,
            //    hdd,
            //    firstRam,
            //    secondRam,
            //    thirdRam,
            //    fourthRam,
            //    fifthRam,
            //    cpu,
            //    powerSupply
            //];

            //var productsDb = products.Select(_mapper.Map<Product>).ToList();

            //_productsRepository.AddRangeAsync(productsDb);
        }
    }
}
