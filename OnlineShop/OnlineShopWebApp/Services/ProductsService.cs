using Newtonsoft.Json;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    //Временный класс для работы с товарами
    public class ProductsService
    {
        public const string FilePath = @".\Data\Products.json";
        private List<Product> _products;

        public ProductsService()
        {
            UpdateProducts();
        }

        public List<Product> GetAll() => _products;

        public Product Get(Guid id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public bool TryGetInfo(Guid id, out string info)
        {
            var product = Get(id);

            if (product is null)
            {
                info = $"Товар с ID:{id} не найден";
                return false;
            }

            info = GetFullInfo(product);
            return true;
        }

        private string GetFullInfo(Product product)
        {
            var baseInfo = $"{product.Id}\n" +
                $"{product.Name}\n" +
                $"{product.Cost}\n" +
                $"{product.Description}\n" +
                $"{product.Category}";

            var specificationsInfo = product.Specifications.Select(spec => $"{spec.Key}: {spec.Value}");

            return $"{baseInfo}\n\nХарактеристики:\n{string.Join("\n", specificationsInfo)}";
        }

        private void UpdateProducts()
        {
            if (!FileService.Exists(FilePath) || string.IsNullOrEmpty(FileService.GetContent(FilePath)))
            {
                InitializeInitialProducts();
            }

            var productsJson = FileService.GetContent(FilePath);

            _products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
        }

        private void InitializeInitialProducts()
        {
            var ssd = new Product("SSD 1Tb Kingston NV2 (SNV2S/1000G)", 7050, "Test Description for SSD", ProductCategories.SSD);
            ssd.Specifications["Manufacturer"] = "Kingston";
            ssd.Specifications["ManufacturerCode"] = "SNV2S/1000G";
            ssd.Specifications["FormFactor"] = "M.2";
            ssd.Specifications["Capacity"] = "1000 Гб";

            var hdd = new Product("2Tb SATA-III Seagate Barracuda (ST2000DM008)", 7030, "Test Description for HDD", ProductCategories.HDD);
            hdd.Specifications["Manufacturer"] = "Seagate";
            hdd.Specifications["ManufacturerCode"] = "ST2000DM008";
            hdd.Specifications["FormFactor"] = "3.5\"";
            hdd.Specifications["Interface"] = "SATA-III";
            hdd.Specifications["Capacity"] = "2000 Гб";

            var ram = new Product("32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategories.RAM);
            ram.Specifications["Manufacturer"] = "Team";
            ram.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            ram.Specifications["FormFactor"] = "DIMM";
            ram.Specifications["MemoryType"] = "DDR5";
            ram.Specifications["MemorySize"] = "32 Гб";
            ram.Specifications["ModulesCount"] = "2";
            ram.Specifications["ClockSpeed"] = "6000 МГц";

            var cpu = new Product("Intel Core i5 - 12400F OEM", 15870, "Test Description for CPU", ProductCategories.Processors);
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

            var products = new List<Product>()
            {
                ssd,
                hdd,
                ram,
                cpu,
                powerSupply
            };

            var jsonData = JsonConvert.SerializeObject(products, Formatting.Indented);
            FileService.Save(FilePath, jsonData);
        }
    }
}
