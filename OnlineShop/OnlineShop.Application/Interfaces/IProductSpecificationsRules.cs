using OnlineShop.Application.Helpers;
using OnlineShop.Db.Models;
using System.Collections.Generic;

namespace OnlineShop.Application.Interfaces
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
