using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IRolesRepository
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        Task<List<Role>> GetAllAsync();

        /// <summary>
        /// Get role by GUID
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="id">Role Id (GUID)</param>
        Task<Role> GetAsync(Guid id);

        /// <summary>
        /// Add role
        /// </summary>
        /// <param name="role">Target role</param>
        Task AddAsync(Role role);

        /// <summary>
        /// Add list of roles
        /// </summary>
        /// <param name="roles">Roles list</param>
        Task AddRangeAsync(List<Role> roles);

        /// <summary>
        /// Delete role by GUID
        /// </summary>
        /// <param name="id">Role Id (GUID)</param>
        Task DeleteAsync(Guid id);
    }
}
