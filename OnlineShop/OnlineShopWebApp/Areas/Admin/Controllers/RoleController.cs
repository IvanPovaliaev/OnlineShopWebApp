using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Db;
using OnlineShopWebApp.Helpers;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class RoleController : Controller
    {
        private readonly IRolesService _rolesService;

        public RoleController(IRolesService rolesService)
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
        public async Task<IActionResult> Add(AddRoleViewModel role)
        {
            var isModelValid = await _rolesService.IsNewValidAsync(ModelState, role);

            if (!isModelValid)
            {
                return PartialView("_AddForm", role);
            }

            await _rolesService.AddAsync(role);

            var redirectUrl = Url.Action(nameof(Index));

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Delete role by name
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="name">Target role name</param>  
        public async Task<IActionResult> Delete(string name)
        {
            await _rolesService.DeleteAsync(name);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Export all roles info to excel
        /// </summary>
        /// <returns>Excel file with roles info</returns>
        public async Task<IActionResult> ExportToExcel()
        {
            var stream = await _rolesService.ExportAllToExcelAsync();

            var downloadFileStream = new FileStreamResult(stream, Formats.ExcelFileContentType)
            {
                FileDownloadName = "Roles.xlsx"
            };

            return downloadFileStream;
        }
    }
}
