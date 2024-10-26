using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

namespace OnlineShopWebApp.Areas.Admin.Views.Product.Components.SpecificationsForm
{
    public class SpecificationsFormViewComponent : ViewComponent
    {
        private readonly ProductsService _productsService;

        public SpecificationsFormViewComponent(ProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Show SpecificationsForm component on View;
        /// </summary>
        /// <returns>SpecificationsFormViewComponent</returns>
        /// <param name="specificationsWithCategory">Tuple with specifications and category</param> 
        public IViewComponentResult Invoke((Dictionary<string, string>, ProductCategoriesViewModel) specificationsWithCategory)
        {
            var specifications = specificationsWithCategory.Item1;
            var category = specificationsWithCategory.Item2;

            var rules = _productsService.GetSpecificationsRules(category);

            var specificationsWithRules = (specifications, rules);

            return View("SpecificationsForm", specificationsWithRules);
        }
    }
}
