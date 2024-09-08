using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private List<Product> _products { get; set; }

        public HomeController()
        {
            var ssd = new Product(1, "SSD 1Tb Kingston NV2 (SNV2S/1000G)", 7050, "Test Description for SSD", ProductCategories.SSD);
            ssd.Specifications["Manufacturer"] = "Kingston";
            ssd.Specifications["ManufacturerCode"] = "SNV2S/1000G";
            ssd.Specifications["FormFactor"] = "M.2";
            ssd.Specifications["Capacity"] = "1000 Ãá";

            var hdd = new Product(2, "2Tb SATA-III Seagate Barracuda (ST2000DM008)", 7030, "Test Description for HDD", ProductCategories.HDD);
            hdd.Specifications["Manufacturer"] = "Seagate";
            hdd.Specifications["ManufacturerCode"] = "ST2000DM008";
            hdd.Specifications["FormFactor"] = "3.5\"";
            hdd.Specifications["Interface"] = "SATA-III";
            hdd.Specifications["Capacity"] = "2000 Ãá";

            var ram = new Product(3, "32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", 11870, "Test Description for RAM", ProductCategories.RAM);
            ram.Specifications["Manufacturer"] = "Team";
            ram.Specifications["ManufacturerCode"] = "CTCED532G6000HC38ADC01";
            ram.Specifications["FormFactor"] = "DIMM";
            ram.Specifications["MemoryType"] = "DDR5";
            ram.Specifications["MemorySize"] = "32 Ãá";
            ram.Specifications["ModulesCount"] = "2";
            ram.Specifications["ClockSpeed"] = "6000 ÌÃö";

            var cpu = new Product(4, "Intel Core i5 - 12400F OEM", 15870, "Test Description for CPU", ProductCategories.Processors);
            cpu.Specifications["Manufacturer"] = "Intel";
            cpu.Specifications["ManufacturerCode"] = "CM8071504555318/CM8071504650609";
            cpu.Specifications["Model"] = "Core i5 12400F";
            cpu.Specifications["Socket"] = "LGA 1700";
            cpu.Specifications["Architecture"] = "Alder Lake";
            cpu.Specifications["CoresCount"] = "6";
            cpu.Specifications["ThreadsCount"] = "12";
            cpu.Specifications["ClockSpeed"] = "2500 ÌÃö";

            var powerSupply = new Product(5, "750W Be Quiet System Power 10", 8250, "Test Description for PowerSupply", ProductCategories.PowerSupplies);
            powerSupply.Specifications["Manufacturer"] = "Be Quiet";
            powerSupply.Specifications["ManufacturerCode"] = "BN329";
            powerSupply.Specifications["Power"] = "750 Âò";
            powerSupply.Specifications["PFC"] = "àêòèâíûé";
            powerSupply.Specifications["FanSize"] = "120 ìì";

            _products = new()
            {
                ssd,
                hdd,
                ram,
                cpu,
                powerSupply
            };
        }

        public IActionResult Index()
        {
            var resultCollection = _products.Select(p => $"{p.Id}\n{p.Name}\n{p.Cost}");
            return Ok(string.Join("\n\n", resultCollection));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
