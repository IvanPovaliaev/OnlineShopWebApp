using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Domain;
using OnlineShop.WebAPI.Helpers;

namespace OnlineShop.WebAPI.Areas.Admin.Controllers
{
	[ApiController]
	[Route("[area]/[controller]")]
	[Area(Constants.AdminRoleName)]
	[Authorize(Roles = Constants.AdminRoleName)]

	public class RoleController : Controller
	{
		private readonly IRolesService _rolesService;

		public RoleController(IRolesService rolesService)
		{
			_rolesService = rolesService;
		}

		/// <summary>
		/// Get all roles
		/// </summary>
		/// <returns>Collections if rolesViewModels</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var roles = await _rolesService.GetAllAsync();
			return Ok(roles);
		}

		/// <summary>
		/// Add new role
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="role">Target role</param>  
		[HttpPost]
		public async Task<IActionResult> Add(AddRoleViewModel role)
		{
			var isModelValid = await _rolesService.IsNewValidAsync(ModelState, role);

			if (!isModelValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			var isSuccess = await _rolesService.AddAsync(role);
			if (isSuccess)
			{
				return Ok(new { Message = $"Role '{role.Name}' added successfully" });
			}

			return StatusCode(500, new { Message = $"Failed to create the role '{role.Name}'. Please try again later." });
		}

		/// <summary>
		/// Delete role by name
		/// </summary>
		/// <returns>Operation StatusCode</returns>
		/// <param name="name">Target role name</param>
		[HttpDelete("{name}")]
		public async Task<IActionResult> Delete(string name)
		{
			var isSuccess = await _rolesService.DeleteAsync(name);

			if (isSuccess)
			{
				return Ok(new { Message = $"Role '{name}' deleted successfully" });
			}

			return NotFound(new { Message = $"Role '{name}' not found or can't be deleted" });
		}
	}
}
