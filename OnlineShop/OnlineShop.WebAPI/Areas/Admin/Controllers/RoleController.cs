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
        /// Open Admin Roles Page
        /// </summary>
        /// <returns>Admin Roles View</returns>
        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _rolesService.GetAllAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <returns>Admin Roles View</returns>
        /// <param name="role">Target role</param>  
        [HttpPost]
        public async Task<IActionResult> Add(AddRoleViewModel role)
        {
            var isModelValid = await _rolesService.IsNewValidAsync(ModelState, role);

            if (!isModelValid)
            {
                return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
            }

            await _rolesService.AddAsync(role);

            var result = new
            {
                Message = $"Role '{role.Name}' added successfully"
            };

            return Ok(result);
        }

        /// <summary>
        /// Delete role by name
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="name">Target role name</param>
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await _rolesService.DeleteAsync(name);

            var result = new
            {
                Message = $"Role '{name}' deleted successfully"
            };

            return Ok(result);
        }
    }
}
