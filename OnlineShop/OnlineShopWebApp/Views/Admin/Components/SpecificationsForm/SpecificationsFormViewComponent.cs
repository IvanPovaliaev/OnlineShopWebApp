using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

namespace OnlineShopWebApp.Views.Admin.Components.SpecificationsForm
{
    public class SpecificationsFormViewComponent : ViewComponent
    {
        private readonly ProductsService _productsService;

        public SpecificationsFormViewComponent(ProductsService productsService)
        {
            _productsService = productsService;
        }

        public IViewComponentResult Invoke((Dictionary<string, string>, ProductCategories category) specificationsWithCategory)
        {
            var specifications = specificationsWithCategory.Item1;
            var category = specificationsWithCategory.Item2;

            var rules = _productsService.GetSpecificationsRules(category);

            var specificationsWithRules = (specifications, rules);

            return View("SpecificationsForm", specificationsWithRules);
        }
    }
}
