using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class FavoriteViewComponent : ViewComponent
    {
        private readonly string? _userId;
        private readonly IFavoritesService _favoritesService;

        public FavoriteViewComponent(IFavoritesService favoritesService, IHttpContextAccessor httpContextAccessor)
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
            var favoritesCount = (await _favoritesService.GetAllAsync(_userId!))
                                                         .Count;

            return View("Favorite", favoritesCount);
        }
    }
}
