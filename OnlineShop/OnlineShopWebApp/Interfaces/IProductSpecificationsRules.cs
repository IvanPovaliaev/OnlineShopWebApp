using OnlineShopWebApp.Helpers;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IProductSpecificationsRules
    {
        /// <summary>
        /// Get all ProductSpecificationRules
        /// </summary>
        /// <returns>List of all specification rules</returns>
        public List<ProductSpecificationRule> GetAll();
    }
}
