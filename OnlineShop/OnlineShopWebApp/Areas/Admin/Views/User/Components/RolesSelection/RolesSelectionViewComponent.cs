using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Views.User.Components.RolesSelection
{
    public class RolesSelectionViewComponent(RolesService rolesService) : ViewComponent
    {
        private readonly RolesService _rolesService = rolesService;

        /// <summary>
        /// Return selection ViewComponent with initial role by selectedRoleId;
        /// </summary>
        /// <returns>RoleSelectionViewComponent</returns>
        /// <param name="selectedRoleId">Target selected role id</param> 
        public async Task<IViewComponentResult> InvokeAsync(Guid selectedRoleId)
        {
            var roles = await _rolesService.GetAllAsync();
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
