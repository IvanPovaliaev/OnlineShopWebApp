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
        /// Get role from repository by id
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="id">Target role id (GUID)</param>
        public virtual async Task<Role?> GetAsync(string id) => await _roleManager.FindByIdAsync(id);

        /// <summary>
        /// Get role from repository by id
        /// </summary>
        /// <returns>RoleViewModel; returns null if role not found</returns>
        /// <param name="id">Target role id (GUID)</param>
        public async Task<RoleViewModel> GetViewModelAsync(string id)
        {
            var roleDb = await GetAsync(id);
            return _mapper.Map<RoleViewModel>(roleDb);
        }

        /// <summary>
        /// Validates the new role model
        /// </summary>        
        /// <returns>true if model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="role">Target role model</param>
        public virtual async Task<bool> IsNewValidAsync(ModelStateDictionary modelState, RoleViewModel role)
        {
            var repositoryRoles = await _roleManager.Roles.ToListAsync();

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
        public virtual async Task AddAsync(RoleViewModel role)
        {
            var roleDb = _mapper.Map<Role>(role);
            await _roleManager.CreateAsync(roleDb);
        }

        /// <summary>
        /// Delete role from repository by id if it can be deleted
        /// </summary>
        /// <param name="id">Target role id (GUID)</param>
        public virtual async Task DeleteAsync(string id)
        {
            var canBeDeleted = await CanBeDeletedAsync(id);
            if (!canBeDeleted)
            {
                return;
            }

            await _mediator.Publish(new RoleDeletedNotification(id));
            var role = await _roleManager.FindByIdAsync(id);
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

        private async Task<bool> CanBeDeletedAsync(string id)
        {
            var role = await GetAsync(id);
            return role.CanBeDeleted;
        }
    }
}
