using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain;
using OnlineShopWebApp.Helpers;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class UserController : Controller
    {
        private readonly IAccountsService _accountsService;

        public UserController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        /// <summary>
        /// Open Admin Users Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        public async Task<IActionResult> Index()
        {
            var users = await _accountsService.GetAllAsync();
            return View(users);
        }

        /// <summary>
        /// Open Admin AddUser Page
        /// </summary>
        /// <returns>Admin AddUser View</returns>
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <returns>Admins users View</returns> 
        /// <param name="register">Target register user model</param>
        [HttpPost]
        public async Task<IActionResult> Add(AdminRegisterViewModel register)
        {
            var isModelValid = await _accountsService.IsRegisterValidAsync(ModelState, register);

            if (!isModelValid)
            {
                return View(nameof(Add), register);
            }

            await _accountsService.AddAsync(register);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Open Admin User Details Page
        /// </summary>
        /// <returns>Admin Users View</returns>
        /// <param name="id">Target user Id</param>
        public async Task<IActionResult> Details(string id)
        {
            var user = await _accountsService.GetAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Change password for user
        /// </summary>
        /// <returns>User Details Page</returns>
        /// <param name="changePassword">Target changePassword model</param>  
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ChangePasswordForm", changePassword);
            }

            await _accountsService.ChangePasswordAsync(changePassword);

            var redirectUrl = Url.Action(nameof(Details), new { id = changePassword.UserId });

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Open Edit User Page
        /// </summary>
        /// <returns>Admin Edit User View</returns>
        /// <param name="editUser">Target EditUser model</param> 
        public IActionResult Edit(AdminEditUserViewModel editUser) => View(editUser);

        /// <summary>
        /// Update target user
        /// </summary>
        /// <returns>User Details Page if success; otherwise Edit User View</returns>
        /// <param name="editUser">Target EditUser model</param>  
        [HttpPost]
        public async Task<IActionResult> Update(AdminEditUserViewModel editUser)
        {
            var isModelValid = await _accountsService.IsEditUserValidAsync(ModelState, editUser);

            if (!isModelValid)
            {
                return View(nameof(Edit), editUser);
            }

            await _accountsService.UpdateInfoAsync(editUser);

            return RedirectToAction(nameof(Details), new { id = editUser.Id });
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <returns>Admins users View</returns>
        /// <param name="id">Target user Id</param>  
        public async Task<IActionResult> Delete(string id)
        {
            await _accountsService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Export all users info to excel
        /// </summary>
        /// <returns>Excel file with users info</returns>
        public async Task<IActionResult> ExportToExcel()
        {
            var stream = await _accountsService.ExportAllToExcelAsync();

            var downloadFileStream = new FileStreamResult(stream, Formats.ExcelFileContentType)
            {
                FileDownloadName = "Users.xlsx"
            };

            return downloadFileStream;
        }
    }
}
