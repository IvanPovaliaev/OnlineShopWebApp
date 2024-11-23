using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Models.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Interfaces
{
    public interface IAccountsService
    {
        /// <summary>
        /// Get all users from repository
        /// </summary>
        /// <returns>List of all UserViewModel from repository</returns>
        Task<List<UserViewModel>> GetAllAsync();

        /// <summary>
        /// Get user from repository by id
        /// </summary>
        /// <returns>UserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id</param>
        Task<UserViewModel> GetAsync(string id);

        /// <summary>
        /// Get EditUserViewModel by id
        /// </summary>
        /// <returns>EditUserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id</param>
        Task<EditUserViewModel> GetEditViewModelAsync(string id);

        /// <summary>
        /// Add a new user to repository based on register info
        /// </summary>        
        /// <param name="register">Target register model</param>
        Task AddAsync(RegisterViewModel register);

        /// <summary>
        /// Change password for related user if user exist
        /// </summary>        
        /// <param name="changePassword">Target ChangePassword model</param>
        Task ChangePasswordAsync(ChangePasswordViewModel changePassword);

        /// <summary>
        /// Update info for related user if user exist
        /// </summary>        
        /// <param name="editUser">Target editUser model</param>
        Task UpdateInfoAsync(EditUserViewModel editUser);

        /// <summary>
        /// Delete user from repository by id. Admin can't be deleted
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Validates the user login model
        /// </summary>        
        /// <returns>true if login model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="login">Target login model</param>
        Task<bool> IsLoginValidAsync(ModelStateDictionary modelState, LoginViewModel login);

        /// <summary>
        /// Validates the registration model
        /// </summary>        
        /// <returns>true if registration model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="register">Target register model</param>
        Task<bool> IsRegisterValidAsync(ModelStateDictionary modelState, RegisterViewModel register);

        /// <summary>
        /// Validates the user edit model
        /// </summary>        
        /// <returns>true if edit model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="editUser">Target edit model</param>
        Task<bool> IsEditUserValidAsync(ModelStateDictionary modelState, EditUserViewModel editUser);

        /// <summary>
        /// Logout user
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Change all users role related to role name to user Role.
        /// </summary>
        /// <param name="oldRoleName">Target old role name</param>
        Task ChangeRolesToUserAsync(string oldRoleName);

        /// <summary>
        /// Get MemoryStream for all users export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        Task<MemoryStream> ExportAllToExcelAsync();

        /// <summary>
        /// Validates the ForgotPasswordViewModel
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="model">Target ForgotPasswordViewModel</param>
        Task<bool> IsForgotPasswordValidAsync(ModelStateDictionary modelState, ForgotPasswordViewModel model);
    }
}
