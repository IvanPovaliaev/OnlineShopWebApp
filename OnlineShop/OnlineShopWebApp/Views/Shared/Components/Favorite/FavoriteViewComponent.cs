using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class FavoriteViewComponent : ViewComponent
    {
        private string? _userId;
        private readonly FavoritesService _favoritesService;

        public FavoriteViewComponent(FavoritesService favoritesService, IHttpContextAccessor httpContextAccessor)
        {
            _favoritesService = favoritesService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show favorite icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var favoritesCount = (await _favoritesService.GetAllAsync(_userId))
                                                         .Count;

            return View("Favorite", favoritesCount);
        }
    }
}
