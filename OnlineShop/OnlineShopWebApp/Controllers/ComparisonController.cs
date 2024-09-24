using Microsoft.AspNetCore.Mvc;
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
        /// Add product to users comparisons.
        /// </summary>
        /// <returns>Users cart View</returns>
        /// <param name="productId">Product id (guid)</param>
        public IActionResult Add(Guid productId)
        {
            _comparisonsService.Create(productId, _userId);
            return RedirectToAction("Index");
        }
    }
}
