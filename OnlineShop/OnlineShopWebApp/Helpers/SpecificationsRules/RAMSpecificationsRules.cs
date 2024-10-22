using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class RAMSpecificationsRules : IProductSpecificationsRules
    {
        public ProductCategories Category => ProductCategories.RAM;

        public List<ProductSpecificationRule> GetAll()
        {
            var formFactorPattern = @"^(DIMM|SO-DIMM)$";
            var formFactorsErrorMessage = @"Неверный форм-фактор. Доступные варианты: DIMM; SO-DIMM;";

            var memoryTypePattern = @"^DDR[2345]$";
            var memoryTypeErrorMessage = @"Неверный тип памяти. Доступные варианты: DDR2; DDR3; DDR4; DDR5";

            var memorySizePattern = @"^\d+\sГб$";
            var memorySizeErrorMessage = @"Неверный объём памяти. Пример: 16 Гб; 32 Гб";

            var modulesCountPattern = @"^\d+$";
            var modulesCountErrorMessage = @"Неверное количество модулей. Пример: 1; 2; 3; 4; 8";

            var clockSpeedPattern = @"^\d+\sМГц$";
            var clockSpeedErrorMessage = @"Неверная тактовая частота. Пример: 3200 МГц";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("FormFactor", formFactorPattern, formFactorsErrorMessage),
                    new("MemoryType", memoryTypePattern, memoryTypeErrorMessage),
                    new("MemorySize", memorySizePattern, memorySizeErrorMessage),
                    new("ModulesCount", modulesCountPattern, modulesCountErrorMessage),
                    new("ClockSpeed", clockSpeedPattern, clockSpeedErrorMessage)
                };

            return rules;
        }
    }
}
