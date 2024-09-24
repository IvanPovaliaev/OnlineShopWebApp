using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Linq;

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
        /// Open comparison page
        /// </summary>
        /// <returns>Comparison page View</returns>
        public IActionResult Index()
        {
            var comparisons = _comparisonsService.GetAll(_userId);
            var groupProducts = comparisons.ToLookup(p => p.Product.Category);

            return View(groupProducts);
        }

        /// <summary>
        /// Open category comparison page
        /// </summary>
        /// <returns>Comparison category page View</returns>
        public IActionResult Category(ProductCategories category)
        {
            var comparisons = _comparisonsService.GetAll(_userId);
            var groupProducts = comparisons.ToLookup(p => p.Product.Category);

            var groupProductWithCategory = (groupProducts, category);

            return View(groupProductWithCategory);
        }

        /// <summary>
        /// Add product to users comparisons.
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="productId">Product id (GUID)</param>
        public IActionResult Add(Guid productId)
        {
            _comparisonsService.Create(productId, _userId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Add product to users comparisons.
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="comparisonId">ComparisonProduct id (GUID)</param>
        public IActionResult Delete(Guid comparisonId)
        {
            _comparisonsService.Delete(comparisonId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete all ComparisonProducts by userId
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="comparisonId">ComparisonProduct id (GUID)</param>
        public IActionResult DeleteAll()
        {
            _comparisonsService.DeleteAll(_userId);
            return RedirectToAction("Index");
        }
    }
}
