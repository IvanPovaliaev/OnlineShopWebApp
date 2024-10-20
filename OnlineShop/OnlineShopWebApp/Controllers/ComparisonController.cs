using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class ComparisonController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly ComparisonsService _comparisonsService;

        public ComparisonController(ComparisonsService comparisonsService)
        {
            _comparisonsService = comparisonsService;
        }

        /// <summary>
        /// Open comparison page of target category. If category is null, open page for first category
        /// </summary>
        /// <returns>Comparison category page View</returns>
        /// <param name="category">Product category</param>
        public IActionResult Index(ProductCategories? category)
        {
            var comparisonsGroups = _comparisonsService.GetGroups(_userId);

            var comparisonsGroupsWithCategory = (comparisonsGroups, category);

            return View(comparisonsGroupsWithCategory);
        }

        /// <summary>
        /// Add product to users comparisons.
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (GUID)</param>
        public IActionResult Add(Guid productId)
        {
            _comparisonsService.Create(productId, _userId);
            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Delete ComparisonProducts from users comparisons by Id.
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="comparisonId">ComparisonProduct id (GUID)</param>
        public IActionResult Delete(Guid comparisonId)
        {
            _comparisonsService.Delete(comparisonId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete all ComparisonProducts from users comparisons
        /// </summary>
        /// <returns>Users comparison View</returns>
        public IActionResult DeleteAll()
        {
            _comparisonsService.DeleteAll(_userId);
            return RedirectToAction("Index");
        }
    }
}
