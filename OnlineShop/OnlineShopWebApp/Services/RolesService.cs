using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.Services
{
    public class RolesService
    {
        private readonly IRolesRepository _rolesRepository;

        public RolesService(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
            InitializeRoles();
        }

        /// <summary>
        /// Get all roles from repository
        /// </summary>
        /// <returns>List of all roles from repository</returns>
        public List<Role> GetAll() => _rolesRepository.GetAll();

        /// <summary>
        /// Get role from repository by id
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="id">Target role id (GUID)</param>
        public Role Get(Guid id) => _rolesRepository.Get(id);

        /// <summary>
        /// Add role to repository
        /// </summary>
        /// <param name="role">Target role</param>
        public void Add(Role role)
        {
            _rolesRepository.Add(role);
        }

        /// <summary>
        /// Delete role from repository by id
        /// </summary>
        /// <param name="id">Target role id (GUID)</param>
        public void Delete(Guid id)
        {
            _rolesRepository.Delete(id);
        }

        /// <summary>
        /// Initializes initial roles if repository is empty;
        /// </summary>
        private void InitializeRoles()
        {
            var roles = _rolesRepository.GetAll();
            if (roles.Count != 0)
            {
                return;
            }

            roles =
            [
                new("Admin"),
                new("User")
            ];

            _rolesRepository.Add(roles);
        }
    }
}
