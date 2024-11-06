using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class RolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IMediator _mediator;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;

        public RolesService(IRolesRepository rolesRepository, IMediator mediator, IMapper mapper, IExcelService excelService)
        {
            _rolesRepository = rolesRepository;
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
            var roles = await _rolesRepository.GetAllAsync();
            return roles.Select(_mapper.Map<RoleViewModel>)
                        .ToList();
        }

        /// <summary>
        /// Get role from repository by id
        /// </summary>
        /// <returns>Role; returns null if role not found</returns>
        /// <param name="id">Target role id (GUID)</param>
        public virtual async Task<Role> GetAsync(Guid id) => await _rolesRepository.GetAsync(id);

        /// <summary>
        /// Get role from repository by id
        /// </summary>
        /// <returns>RoleViewModel; returns null if role not found</returns>
        /// <param name="id">Target role id (GUID)</param>
        public async Task<RoleViewModel> GetViewModelAsync(Guid id)
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
        public async Task<bool> IsNewValidAsync(ModelStateDictionary modelState, RoleViewModel role)
        {
            var repositoryRoles = await _rolesRepository.GetAllAsync();

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
        public async Task AddAsync(RoleViewModel role)
        {
            var roleDb = _mapper.Map<Role>(role);
            await _rolesRepository.AddAsync(roleDb);
        }

        /// <summary>
        /// Delete role from repository by id if it can be deleted
        /// </summary>
        /// <param name="id">Target role id (GUID)</param>
        public async Task DeleteAsync(Guid id)
        {
            var canBeDeleted = await CanBeDeletedAsync(id);
            if (!canBeDeleted)
            {
                return;
            }

            await _mediator.Publish(new RoleDeletedNotification(id));
            await _rolesRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Get MemoryStream for all roles export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with roles info</returns>
        public async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var roles = await GetAllAsync();
            return _excelService.ExportRoles(roles);
        }

        private async Task<bool> CanBeDeletedAsync(Guid id)
        {
            var role = await GetAsync(id);
            return role.CanBeDeleted;
        }
    }
}
