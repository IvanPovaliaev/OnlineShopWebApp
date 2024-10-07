using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers.SpecificationsRules
{
    public class ProcessorSpecificationsRules : IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll()
        {
            var rules = new List<ProductSpecificationRule>
                {
                    new("Manufacturer"),
                    new("ManufacturerCode"),
                    new("Model"),
                    new("Socket"),
                    new("Architecture"),
                    new("CoresCount"),
                    new("ThreadsCount"),
                    new("ClockSpeed")
                };

            return rules;
        }
    }
}
