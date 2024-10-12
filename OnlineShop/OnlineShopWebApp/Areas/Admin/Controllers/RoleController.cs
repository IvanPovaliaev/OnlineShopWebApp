using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RolesService _rolesService;

        public RoleController(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        /// <summary>
        /// Open Admin Roles Page
        /// </summary>
        /// <returns>Admin Roles View</returns>
        public IActionResult Index()
        {
            var roles = _rolesService.GetAll();
            return View(roles);
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <returns>Admin Roles View</returns>
        [HttpPost]
        public IActionResult AddRole(Role role)
        {
            var isModelValid = _rolesService.IsNewValid(ModelState, role);

            if (!isModelValid)
            {
                return PartialView("_AddRoleForm", role);
            }

            _rolesService.Add(role);

            var redirectUrl = Url.Action("Index");

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Delete role by Id
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="roleId">Target roleId</param>  
        public IActionResult DeleteRole(Guid roleId)
        {
            _rolesService.Delete(roleId);
            return RedirectToAction("Index");
        }
    }
}
