using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class RolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IMediator _mediator;
        private readonly IExcelService _excelService;

        public RolesService(IRolesRepository rolesRepository, IMediator mediator, IExcelService excelService)
        {
            _rolesRepository = rolesRepository;
            _mediator = mediator;
            _excelService = excelService;
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
        public void Add(Role role) => _rolesRepository.Add(role);

        /// <summary>
        /// Delete role from repository by id if it can be deleted
        /// </summary>
        /// <param name="id">Target role id (GUID)</param>
        public void Delete(Guid id)
        {
            if (!CanBeDeleted(id))
            {
                return;
            }

            _mediator.Publish(new RoleDeletedNotification(id));
            _rolesRepository.Delete(id);
        }

        /// <summary>
        /// Get MemoryStream for all roles export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with roles info</returns>
        public MemoryStream ExportAllToExcel()
        {
            var roles = GetAll();
            return _excelService.ExportRoles(roles);
        }

        private bool CanBeDeleted(Guid id)
        {
            var role = _rolesRepository.Get(id);
            return role.CanBeDeleted;
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
                new(Constants.AdminRoleName)
                {
                    CanBeDeleted = false
                },
                new(Constants.UserRoleName)
                {
                    CanBeDeleted = false
                }
            ];

            _rolesRepository.Add(roles);
        }
    }
}
