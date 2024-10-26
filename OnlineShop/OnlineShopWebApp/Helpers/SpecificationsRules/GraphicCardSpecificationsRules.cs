using OnlineShop.Db.Models;
using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class GraphicCardSpecificationsRules : IProductSpecificationsRules
    {
        public ProductCategories Category => ProductCategories.GraphicCards;

        public List<ProductSpecificationRule> GetAll()
        {
            var interfacePattern = @"^(PCIe (2.0|3.0|4.0))$";
            var interfaceErrorMessage = @"Неверный тип интерфейса. Доступные варианты: PCIe 2.0; PCIe 3.0; PCIe 4.0";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("Interface", interfacePattern, interfaceErrorMessage)
                };

            return rules;
        }
    }
}
