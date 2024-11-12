using Microsoft.AspNetCore.Mvc;
using OnlineShop.Db;
using OnlineShopWebApp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Views.User.Components.RolesSelection
{
    public class RolesSelectionViewComponent(RolesService rolesService) : ViewComponent
    {
        private readonly RolesService _rolesService = rolesService;

        /// <summary>
        /// Return selection ViewComponent with initial role by selectedRoleName;
        /// </summary>
        /// <returns>RoleSelectionViewComponent</returns>
        /// <param name="selectedRoleName">Target selected role name</param> 
        public async Task<IViewComponentResult> InvokeAsync(string selectedRoleName)
        {
            var roles = await _rolesService.GetAllAsync();
            var defaultRoleName = Constants.UserRoleName;

            if (roles.Any(r => r.Name == selectedRoleName))
            {
                defaultRoleName = selectedRoleName;
            }

            var rolesWithDefaultName = (roles, defaultRoleName);

            return View("RolesSelection", rolesWithDefaultName);
        }
    }
}
