using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    //Временный класс для работы с товарами
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
            return GetAll().Where(p => p.Category == category).ToList();
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

            var ramImageUrl = "/img/products/32Gb-DDR5-6000MHz-Team-T-Create-Expert-_2x16Gb-KIT.webp";
            var ram = new Product("32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategories.RAM, ramImageUrl);
            ram.Specifications["Manufacturer"] = "Team";
            ram.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            ram.Specifications["FormFactor"] = "DIMM";
            ram.Specifications["MemoryType"] = "DDR5";
            ram.Specifications["MemorySize"] = "32 Гб";
            ram.Specifications["ModulesCount"] = "2";
            ram.Specifications["ClockSpeed"] = "6000 МГц";

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
                ram,
                cpu,
                powerSupply
            ];

            _productsRepository.Add(products);
        }
    }
}
