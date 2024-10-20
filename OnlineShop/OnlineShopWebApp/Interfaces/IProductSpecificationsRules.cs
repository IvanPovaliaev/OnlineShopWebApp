using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IProductSpecificationsRules
    {
        ProductCategories Category { get; }

        /// <summary>
        /// Get all ProductSpecificationRules
        /// </summary>
        /// <returns>List of all specification rules</returns>
        List<ProductSpecificationRule> GetAll();
    }
}
