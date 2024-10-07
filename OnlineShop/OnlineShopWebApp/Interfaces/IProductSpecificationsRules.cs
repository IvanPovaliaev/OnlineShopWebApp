using System.Collections.Generic;
using OnlineShopWebApp.Helpers;

namespace OnlineShopWebApp.Interfaces
{
    public interface IProductSpecificationsRules
    {
        public List<ProductSpecificationRule> GetAll();
    }
}
