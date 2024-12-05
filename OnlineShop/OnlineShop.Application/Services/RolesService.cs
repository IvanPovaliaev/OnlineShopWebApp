using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Helpers.Notifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
	public class RolesService : IRolesService
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

		public async Task<List<RoleViewModel>> GetAllAsync()
		{
			var roles = await _roleManager.Roles.ToListAsync();
			return roles.Select(_mapper.Map<RoleViewModel>)
						.ToList();
		}

		public async Task<Role?> GetAsync(string name) => await _roleManager.FindByNameAsync(name);

		public async Task<bool> IsNewValidAsync(ModelStateDictionary modelState, AddRoleViewModel role)
		{
			var repositoryRoles = await _roleManager.Roles.ToListAsync();

			if (repositoryRoles.Any(r => r.Name?.ToLower() == role.Name.ToLower()))
			{
				modelState.AddModelError(string.Empty, "Роль с таким именем уже существует!");
			}

			return modelState.IsValid;
		}

		public async Task<bool> AddAsync(AddRoleViewModel role)
		{
			var roleDb = _mapper.Map<Role>(role);

			var result = await _roleManager.CreateAsync(roleDb);
			return result.Succeeded;
		}

		public async Task<bool> DeleteAsync(string name)
		{
			var canBeDeleted = await CanBeDeletedAsync(name);
			if (!canBeDeleted)
			{
				return false;
			}

			await _mediator.Publish(new RoleDeletedNotification(name));

			var role = await GetAsync(name);
			var result = await _roleManager.DeleteAsync(role!);
			return result.Succeeded;
		}

		public async Task<MemoryStream> ExportAllToExcelAsync()
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
