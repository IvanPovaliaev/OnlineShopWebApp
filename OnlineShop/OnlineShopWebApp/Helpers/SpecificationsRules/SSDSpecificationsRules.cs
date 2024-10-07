using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class SSDSpecificationsRules : IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll()
        {
            var formFactorPattern = @"^(2\.5""|M\.2 (2280|2242|2260|22110)|mSATA|U\.2|PCI-e)$";
            var formFactorsErrorMessage = @"Неверный форм-фактор. Доступные варианты: 2.5""; M.2 2280; M.2 2242; M.2 2260; M.2 22110; mSATA; U.2;";

            var capacityPattern = @"^\d+\sГб$";
            var capacityErrorMessage = @"Неверный объём. Пример: 256 Гб; 512 Гб";

            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("FormFactor", formFactorPattern, formFactorsErrorMessage),
                    new("Capacity", capacityPattern, capacityErrorMessage)
                };

            return rules;
        }
    }
}
