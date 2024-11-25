﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class AvatarViewComponent : ViewComponent
    {
        private readonly string _userId;
        private readonly IAccountsService _accountsService;

        public AvatarViewComponent(IAccountsService accountsService, IHttpContextAccessor httpContextAccessor)
        {
            _accountsService = accountsService;
            _userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Show avatar component on View;
        /// </summary>
        /// <returns>AvatarViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _accountsService.GetAsync(_userId);
            var avatarUrl = user?.AvatarUrl;
            return View("Avatar", avatarUrl);
        }
    }
}