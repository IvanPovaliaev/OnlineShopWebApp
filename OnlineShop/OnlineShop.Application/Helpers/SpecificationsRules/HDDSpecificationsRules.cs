using OnlineShop.Application.Interfaces;
using OnlineShop.Domain.Models;
using System.Collections.Generic;

namespace OnlineShop.Application.Helpers.SpecificationsRules
{
    public class HDDSpecificationsRules : IProductSpecificationsRules
    {
        public ProductCategories Category => ProductCategories.HDD;

        public List<ProductSpecificationRule> GetAll()
        {
            var formFactorPattern = @"^(1\.8""|2\.5""|3\.5"")$";
            var formFactorsErrorMessage = @"Неверный форм-фактор. Доступные варианты: 2.5""; 3.5""";

            var interfacePattern = @"^SATA-(II|III)$";
            var interfaceErrorMessage = @"Неверный тип интерфейса. Доступные варианты: SATA-II; SATA-III";

            var capacityPattern = @"^\d+\sГб$";
            var capacityErrorMessage = @"Неверный объём. Пример: 256 Гб; 512 Гб";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("FormFactor", formFactorPattern, formFactorsErrorMessage),
                    new("Interface", interfacePattern, interfaceErrorMessage),
                    new("Capacity", capacityPattern, capacityErrorMessage)
                };

            return rules;
        }
    }

}
