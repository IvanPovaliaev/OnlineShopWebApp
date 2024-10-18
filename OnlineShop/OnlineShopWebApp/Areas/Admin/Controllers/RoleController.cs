using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Helpers;
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
        /// <param name="role">Target role</param>  
        [HttpPost]
        public IActionResult Add(Role role)
        {
            var isModelValid = _rolesService.IsNewValid(ModelState, role);

            if (!isModelValid)
            {
                return PartialView("_AddForm", role);
            }

            _rolesService.Add(role);

            var redirectUrl = Url.Action("Index");

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Delete role by Id
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="id">Target role Id</param>  
        public IActionResult Delete(Guid id)
        {
            _rolesService.Delete(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Export all roles info to excel
        /// </summary>
        /// <returns>Excel file with roles info</returns>
        public IActionResult ExportToExcel()
        {
            var stream = _rolesService.ExportAllToExcel();

            var downloadFileStream = new FileStreamResult(stream, Constants.ExcelFileContentType)
            {
                FileDownloadName = "Roles.xlsx"
            };

            return downloadFileStream;
        }
    }
}
