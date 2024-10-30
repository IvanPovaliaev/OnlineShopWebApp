using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Services;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var roles = await _rolesService.GetAllAsync();
            return View(roles);
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <returns>Admin Roles View</returns>
        /// <param name="role">Target role</param>  
        [HttpPost]
        public async Task<IActionResult> Add(RoleViewModel role)
        {
            var isModelValid = await _rolesService.IsNewValidAsync(ModelState, role);

            if (!isModelValid)
            {
                return PartialView("_AddForm", role);
            }

            await _rolesService.AddAsync(role);

            var redirectUrl = Url.Action("Index");

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Delete role by Id
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="id">Target role Id</param>  
        public async Task<IActionResult> Delete(Guid id)
        {
            await _rolesService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Export all roles info to excel
        /// </summary>
        /// <returns>Excel file with roles info</returns>
        public async Task<IActionResult> ExportToExcel()
        {
            var stream = await _rolesService.ExportAllToExcelAsync();

            var downloadFileStream = new FileStreamResult(stream, Constants.ExcelFileContentType)
            {
                FileDownloadName = "Roles.xlsx"
            };

            return downloadFileStream;
        }
    }
}
