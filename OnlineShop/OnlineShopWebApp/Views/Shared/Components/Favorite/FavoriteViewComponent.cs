﻿using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class FavoriteViewComponent : ViewComponent
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования
        private readonly FavoritesService _favoritesService;

        public FavoriteViewComponent(FavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        /// <summary>
        /// Show favorite icon component on View;
        /// </summary>
        /// <returns>CartViewComponent</returns>
        public IViewComponentResult Invoke()
        {
            var favoritesCount = _favoritesService
                .GetAll(_userId)
                .Count;

            return View("Favorite", favoritesCount);
        }
    }
}
