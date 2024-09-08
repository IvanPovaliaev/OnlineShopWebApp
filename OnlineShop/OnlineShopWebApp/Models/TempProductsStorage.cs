using System.Collections;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    //Временны класс для хранения товаров
    public class TempProductsStorage
    {
        public IEnumerable<Product> GetAll()
        {
            var ssd = new Product(1, "SSD 1Tb Kingston NV2 (SNV2S/1000G)", 7050, "Test Description for SSD", ProductCategories.SSD);
            ssd.Specifications["Manufacturer"] = "Kingston";
            ssd.Specifications["ManufacturerCode"] = "SNV2S/1000G";
            ssd.Specifications["FormFactor"] = "M.2";
            ssd.Specifications["Capacity"] = "1000 Гб";

            var hdd = new Product(2, "2Tb SATA-III Seagate Barracuda (ST2000DM008)", 7030, "Test Description for HDD", ProductCategories.HDD);
            hdd.Specifications["Manufacturer"] = "Seagate";
            hdd.Specifications["ManufacturerCode"] = "ST2000DM008";
            hdd.Specifications["FormFactor"] = "3.5\"";
            hdd.Specifications["Interface"] = "SATA-III";
            hdd.Specifications["Capacity"] = "2000 Гб";

            var ram = new Product(3, "32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategories.RAM);
            ram.Specifications["Manufacturer"] = "Team";
            ram.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            ram.Specifications["FormFactor"] = "DIMM";
            ram.Specifications["MemoryType"] = "DDR5";
            ram.Specifications["MemorySize"] = "32 Гб";
            ram.Specifications["ModulesCount"] = "2";
            ram.Specifications["ClockSpeed"] = "6000 МГц";

            var cpu = new Product(4, "Intel Core i5 - 12400F OEM", 15870, "Test Description for CPU", ProductCategories.Processors);
            cpu.Specifications["Manufacturer"] = "Intel";
            cpu.Specifications["ManufacturerCode"] = "CM8071504555318/CM8071504650609";
            cpu.Specifications["Model"] = "Core i5 12400F";
            cpu.Specifications["Socket"] = "LGA 1700";
            cpu.Specifications["Architecture"] = "Alder Lake";
            cpu.Specifications["CoresCount"] = "6";
            cpu.Specifications["ThreadsCount"] = "12";
            cpu.Specifications["ClockSpeed"] = "2500 МГц";

            var powerSupply = new Product(5, "750W Be Quiet System Power 10", 8250, "Test Description for PowerSupply", ProductCategories.PowerSupplies);
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
            return products;
        }
    }
}
