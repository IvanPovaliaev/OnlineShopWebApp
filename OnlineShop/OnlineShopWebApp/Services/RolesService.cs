using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Validates the new role model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="role">Target role model</param>
        public bool IsNewValid(ModelStateDictionary modelState, Role role)
        {
            var repositoryRoles = GetAll();

            if (repositoryRoles.Any(r => r.Name.ToLower() == role.Name.ToLower()))
            {
                modelState.AddModelError(string.Empty, "Роль с таким именем уже существует!");
            }

            return modelState.IsValid;
        }

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
                new(Constants.AdminRoleName),
                new(Constants.UserRoleName)
            ];

            _rolesRepository.Add(roles);
        }
    }
}
