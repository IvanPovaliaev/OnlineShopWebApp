using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class ComparisonController : Controller
    {
        private readonly string? _userId;
        private readonly IComparisonsService _comparisonsService;

        public ComparisonController(IComparisonsService comparisonsService, IHttpContextAccessor httpContextAccessor)
        {
            _comparisonsService = comparisonsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Open comparison page of target category. If category is null, open page for first category
        /// </summary>
        /// <returns>Comparison category page View</returns>
        /// <param name="category">Product category</param>
        public async Task<IActionResult> Index(ProductCategoriesViewModel? category)
        {
            var comparisonsGroups = await _comparisonsService.GetGroupsAsync(_userId!);

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
            await _comparisonsService.CreateAsync(productId, _userId!);
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
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete all ComparisonProducts from users comparisons
        /// </summary>
        /// <returns>Users comparison View</returns>
        public async Task<IActionResult> DeleteAll()
        {
            await _comparisonsService.DeleteAllAsync(_userId!);
            return RedirectToAction(nameof(Index));
        }
    }
}
