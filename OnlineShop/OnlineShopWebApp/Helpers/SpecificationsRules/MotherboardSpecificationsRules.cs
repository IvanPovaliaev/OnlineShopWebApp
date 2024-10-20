using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class MotherboardSpecificationsRules : IProductSpecificationsRules
    {
        public ProductCategories Category => ProductCategories.Motherboards;

        public List<ProductSpecificationRule> GetAll()
        {
            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("Socket"),
                    new("Chipset")
                };

            return rules;
        }
    }
}
