using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class ProductsService
    {
        private IProductsRepository _productsRepository;

        public ProductsService(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
            InitializeProducts();
        }

        /// <summary>
        /// Get all products from repository
        /// </summary>
        /// <returns>List of all products from repository</returns>
        public List<Product> GetAll() => _productsRepository.GetAll();

        /// <summary>
        /// Get all products from repository for current category
        /// </summary>        
        /// <returns>List of all products from repository for current category</returns>
        /// <param name="category">Product category</param>
        public List<Product> GetAll(ProductCategories category)
        {
            var products = GetAll()
                .Where(p => p.Category == category)
                .ToList();
            return products;
        }

        /// <summary>
        /// Get product from repository by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        public Product Get(Guid id)
        {
            var product = _productsRepository.Get(id);
            return product;
        }

        /// <summary>
        /// Add product to repository
        /// </summary>
        public void Add(Product product)
        {
            _productsRepository.Add(product);
        }

        /// <summary>
        /// Delete product from repository by GUID
        /// </summary>
        public void Delete(Guid id)
        {
            _productsRepository.Delete(id);
        }

        /// <summary>
        /// Initializes initial products if repository is empty;
        /// </summary>
        private void InitializeProducts()
        {
            var products = _productsRepository.GetAll();
            if (products.Count != 0)
            {
                return;
            }

            var ssdImageUrl = "/img/products/SSD-1Tb-Kingston-NV2.webp";
            var ssd = new Product("SSD 1Tb Kingston NV2 (SNV2S/1000G)", 7050, "Test Description for SSD", ProductCategories.SSD, ssdImageUrl);
            ssd.Specifications["Manufacturer"] = "Kingston";
            ssd.Specifications["ManufacturerCode"] = "SNV2S/1000G";
            ssd.Specifications["FormFactor"] = "M.2";
            ssd.Specifications["Capacity"] = "1000 Гб";

            var hddImageUrl = "/img/products/2Tb-SATA-III-Seagate-Barracuda.webp";
            var hdd = new Product("2Tb SATA-III Seagate Barracuda (ST2000DM008)", 7030, "Test Description for HDD", ProductCategories.HDD, hddImageUrl);
            hdd.Specifications["Manufacturer"] = "Seagate";
            hdd.Specifications["ManufacturerCode"] = "ST2000DM008";
            hdd.Specifications["FormFactor"] = "3.5\"";
            hdd.Specifications["Interface"] = "SATA-III";
            hdd.Specifications["Capacity"] = "2000 Гб";

            var firstRamImageUrl = "/img/products/32Gb-DDR5-6000MHz-Team-T-Create-Expert-_2x16Gb-KIT.webp";
            var firstRam = new Product("32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategories.RAM, firstRamImageUrl);
            firstRam.Specifications["Manufacturer"] = "Team";
            firstRam.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            firstRam.Specifications["FormFactor"] = "DIMM";
            firstRam.Specifications["MemoryType"] = "DDR5";
            firstRam.Specifications["MemorySize"] = "32 Гб";
            firstRam.Specifications["ModulesCount"] = "2";
            firstRam.Specifications["ClockSpeed"] = "6000 МГц";

            var secondRamImageUrl = "/img/products/32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT).webp";
            var secondRam = new Product("32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT)", 14160, "Test Description for RAM", ProductCategories.RAM, secondRamImageUrl);
            secondRam.Specifications["Manufacturer"] = "Kingston";
            secondRam.Specifications["ManufacturerCode"] = "KF560C40BBK2-32";
            secondRam.Specifications["FormFactor"] = "DIMM";
            secondRam.Specifications["MemoryType"] = "DDR5";
            secondRam.Specifications["MemorySize"] = "32 Гб";
            secondRam.Specifications["ModulesCount"] = "2";
            secondRam.Specifications["ClockSpeed"] = "6000 МГц";

            var thirdRamImageUrl = "/img/products/16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT).webp";
            var thirdRam = new Product("16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT)", 3970, "Test Description for RAM", ProductCategories.RAM, thirdRamImageUrl);
            thirdRam.Specifications["Manufacturer"] = "Netac";
            thirdRam.Specifications["ManufacturerCode"] = "NTSWD4P32DP-16W";
            thirdRam.Specifications["FormFactor"] = "DIMM";
            thirdRam.Specifications["MemoryType"] = "DDR4";
            thirdRam.Specifications["MemorySize"] = "16 Гб";
            thirdRam.Specifications["ModulesCount"] = "2";
            thirdRam.Specifications["ClockSpeed"] = "3200 МГц";

            var fourthRamImageUrl = "/img/products/32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT).webp";
            var fourthRam = new Product("32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT)", 8450, "Test Description for RAM", ProductCategories.RAM, fourthRamImageUrl);
            fourthRam.Specifications["Manufacturer"] = "Patriot MemoryPatriot Memory";
            fourthRam.Specifications["ManufacturerCode"] = "PVSR432G360C0K";
            fourthRam.Specifications["FormFactor"] = "DIMM";
            fourthRam.Specifications["MemoryType"] = "DDR4";
            fourthRam.Specifications["MemorySize"] = "32 Гб";
            fourthRam.Specifications["ModulesCount"] = "2";
            fourthRam.Specifications["ClockSpeed"] = "3600 МГц";

            var fifthRamImageUrl = "/img/products/64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT).webp";
            var fifthRam = new Product("64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT)", 20790, "Test Description for RAM", ProductCategories.RAM, fifthRamImageUrl);
            fifthRam.Specifications["Manufacturer"] = "ADATA";
            fifthRam.Specifications["ManufacturerCode"] = "AX5U5600C3632G-DCLABK";
            fifthRam.Specifications["FormFactor"] = "DIMM";
            fifthRam.Specifications["MemoryType"] = "DDR5";
            fifthRam.Specifications["MemorySize"] = "64 Гб";
            fifthRam.Specifications["ModulesCount"] = "2";
            fifthRam.Specifications["ClockSpeed"] = "5600 МГц";

            var cpuImageUrl = "/img/products/Intel-Core-i5-12400F-OEM.webp";
            var cpu = new Product("Intel Core i5 - 12400F OEM", 15870, "Test Description for CPU", ProductCategories.Processors, cpuImageUrl);
            cpu.Specifications["Manufacturer"] = "Intel";
            cpu.Specifications["ManufacturerCode"] = "CM8071504555318/CM8071504650609";
            cpu.Specifications["Model"] = "Core i5 12400F";
            cpu.Specifications["Socket"] = "LGA 1700";
            cpu.Specifications["Architecture"] = "Alder Lake";
            cpu.Specifications["CoresCount"] = "6";
            cpu.Specifications["ThreadsCount"] = "12";
            cpu.Specifications["ClockSpeed"] = "2500 МГц";

            var powerSupply = new Product("750W Be Quiet System Power 10", 8250, "Test Description for PowerSupply", ProductCategories.PowerSupplies);
            powerSupply.Specifications["Manufacturer"] = "Be Quiet";
            powerSupply.Specifications["ManufacturerCode"] = "BN329";
            powerSupply.Specifications["Power"] = "750 Вт";
            powerSupply.Specifications["PFC"] = "активный";
            powerSupply.Specifications["FanSize"] = "120 мм";

            products =
            [
                ssd,
                hdd,
                firstRam,
                secondRam,
                thirdRam,
                fourthRam,
                fifthRam,
                cpu,
                powerSupply
            ];

            _productsRepository.Add(products);
        }
    }
}
