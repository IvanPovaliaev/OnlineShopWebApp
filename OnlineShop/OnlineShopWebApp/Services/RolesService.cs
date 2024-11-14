using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class RolesService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMediator _mediator;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;

        public RolesService(RoleManager<Role> roleManager, IMediator mediator, IMapper mapper, IExcelService excelService)
        {
            _roleManager = roleManager;
            _mediator = mediator;
            _excelService = excelService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all roles from repository
        /// </summary>
        /// <returns>List of all RolesViewModel from repository</returns>
        public virtual async Task<List<RoleViewModel>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(_mapper.Map<RoleViewModel>)
                        .ToList();
        }

        /// <summary>
        /// Get role from repository by name
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="name">Target role name</param>
        public virtual async Task<Role?> GetAsync(string name) => await _roleManager.FindByNameAsync(name);

        /// <summary>
        /// Validates the new role model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="role">Target role model</param>
        public virtual async Task<bool> IsNewValidAsync(ModelStateDictionary modelState, AddRoleViewModel role)
        {
            var repositoryRoles = await _roleManager.Roles.ToListAsync();

            if (repositoryRoles.Any(r => r.Name?.ToLower() == role.Name.ToLower()))
            {
                modelState.AddModelError(string.Empty, "Роль с таким именем уже существует!");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Add role to repository
        /// </summary>
        /// <param name="role">Target role</param>
        public virtual async Task AddAsync(AddRoleViewModel role)
        {
            var roleDb = _mapper.Map<Role>(role);
            await _roleManager.CreateAsync(roleDb);
        }

        /// <summary>
        /// Delete role from repository by id if it can be deleted
        /// </summary>
        /// <param name="name">Target role id (GUID)</param>
        public virtual async Task DeleteAsync(string name)
        {
            var canBeDeleted = await CanBeDeletedAsync(name);
            if (!canBeDeleted)
            {
                return;
            }

            await _mediator.Publish(new RoleDeletedNotification(name));

            var role = await GetAsync(name);
            await _roleManager.DeleteAsync(role!);
        }

        /// <summary>
        /// Get MemoryStream for all roles export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with roles info</returns>
        public virtual async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var roles = await GetAllAsync();
            return _excelService.ExportRoles(roles);
        }

        /// <summary>
        /// Checks if a role with the given name can be deleted.
        /// </summary>        
        /// <returns>true if can; otherwise false</returns>
        /// <param name="name">Target role name</param>
        private async Task<bool> CanBeDeletedAsync(string name)
        {
            var role = await GetAsync(name);
            return role?.CanBeDeleted ?? false;
        }
    }
}
