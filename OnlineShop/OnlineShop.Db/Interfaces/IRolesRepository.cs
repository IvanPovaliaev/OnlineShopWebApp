using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Interfaces
{
    public interface IRolesRepository
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        List<Role> GetAll();

        /// <summary>
        /// Get role by GUID
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="id">Role Id (GUID)</param>
        Role Get(Guid id);

        /// <summary>
        /// Add role
        /// </summary>
        /// <param name="role">Target role</param>
        void Add(Role role);

        /// <summary>
        /// Add list of roles
        /// </summary>
        /// <param name="roles">Roles list</param>
        void Add(List<Role> roles);

        /// <summary>
        /// Delete role by GUID
        /// </summary>
        /// <param name="id">Role Id (GUID)</param>
        void Delete(Guid id);
    }
}
