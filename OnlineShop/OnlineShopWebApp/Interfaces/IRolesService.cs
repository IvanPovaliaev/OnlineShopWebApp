using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Interfaces
{
    public interface IRolesService
    {
        /// <summary>
        /// Get all roles from repository
        /// </summary>
        /// <returns>List of all RolesViewModel from repository</returns>
        Task<List<RoleViewModel>> GetAllAsync();

        /// <summary>
        /// Get role from repository by name
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="name">Target role name</param>
        Task<Role?> GetAsync(string name);

        /// <summary>
        /// Validates the new role model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="role">Target role model</param>
        Task<bool> IsNewValidAsync(ModelStateDictionary modelState, AddRoleViewModel role);

        /// <summary>
        /// Add role to repository
        /// </summary>
        /// <param name="role">Target role</param>
        Task AddAsync(AddRoleViewModel role);

        /// <summary>
        /// Delete role from repository by id if it can be deleted
        /// </summary>
        /// <param name="name">Target role id (GUID)</param>
        Task DeleteAsync(string name);

        /// <summary>
        /// Get MemoryStream for all roles export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with roles info</returns>
        Task<MemoryStream> ExportAllToExcelAsync();
    }
}
