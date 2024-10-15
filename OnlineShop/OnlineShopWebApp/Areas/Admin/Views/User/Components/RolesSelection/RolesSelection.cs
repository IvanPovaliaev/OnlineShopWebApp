using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Services;
using System;
using System.Linq;

namespace OnlineShopWebApp.Areas.Admin.Views.User.Components.RolesSelection
{
    public class RolesSelection : ViewComponent
    {
        private readonly RolesService _rolesService;

        public RolesSelection(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        /// <summary>
        /// Show SpecificationsForm component on View;
        /// </summary>
        /// <returns>SpecificationsFormViewComponent</returns>
        /// <param name="specificationsWithCategory">Tuple with specifications and category</param> 
        public IViewComponentResult Invoke(Guid selectedRoleId)
        {
            var roles = _rolesService.GetAll();
            var defaultRoleId = roles.First(r => r.Name == Constants.UserRoleName).Id;

            if (roles.Any(r => r.Id == selectedRoleId))
            {
                defaultRoleId = selectedRoleId;
            }

            var rolesWithDefaultId = (roles, defaultRoleId);

            return View("RolesSelection", rolesWithDefaultId);
        }
    }
}
