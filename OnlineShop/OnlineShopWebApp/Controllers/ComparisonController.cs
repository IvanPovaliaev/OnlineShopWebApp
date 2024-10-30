using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(ProductCategoriesViewModel? category)
        {
            var comparisonsGroups = await _comparisonsService.GetGroupsAsync(_userId);

            var comparisonsGroupsWithCategory = (comparisonsGroups, category);

            return View(comparisonsGroupsWithCategory);
        }

        /// <summary>
        /// Add product to users comparisons.
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (GUID)</param>
        public async Task<IActionResult> Add(Guid productId)
        {
            await _comparisonsService.CreateAsync(productId, _userId);
            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Delete ComparisonProducts from users comparisons by Id.
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="id">ComparisonProduct id (GUID)</param>
        public async Task<IActionResult> Delete(Guid id)
        {
            await _comparisonsService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete all ComparisonProducts from users comparisons
        /// </summary>
        /// <returns>Users comparison View</returns>
        public async Task<IActionResult> DeleteAll()
        {
            await _comparisonsService.DeleteAllAsync(_userId);
            return RedirectToAction("Index");
        }
    }
}
