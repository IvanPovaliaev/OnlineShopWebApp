using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class RAMSpecificationsRules : IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll()
        {
            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("FormFactor"),
                    new("MemoryType"),
                    new("MemorySize"),
                    new("ModulesCount"),
                    new("ClockSpeed")
                };

            return rules;
        }
    }
}
