using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Controllers
{
    public class FavoriteController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly FavoritesService _favoritesService;

        public FavoriteController(FavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        /// <summary>
        /// Open favorite page
        /// </summary>
        /// <returns>Users favorites View</returns>
        public async Task<IActionResult> Index()
        {
            var favorites = await _favoritesService.GetAllAsync(_userId);
            return View(favorites);
        }

        /// <summary>
        /// Add product to users favorites.
        /// </summary>
        /// <returns>_NavUserIcons PartialView</returns>
        /// <param name="productId">Product id (GUID)</param>
        public async Task<IActionResult> Add(Guid productId)
        {
            await _favoritesService.CreateAsync(productId, _userId);
            return PartialView("_NavUserIcons");
        }

        /// <summary>
        /// Delete product from users favorites
        /// </summary>
        /// <returns>Users comparison View</returns>
        /// <param name="id">FavoriteProduct Id (GUID)</param>
        public async Task<IActionResult> Delete(Guid id)
        {
            await _favoritesService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete all FavoriteProducts by userId
        /// </summary>
        /// <returns>Users comparison View</returns>
        public async Task<IActionResult> DeleteAll()
        {
            await _favoritesService.DeleteAllAsync(_userId);
            return RedirectToAction("Index");
        }
    }
}
