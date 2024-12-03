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
	public class UserController : Controller
	{
		private readonly IAccountsService _accountsService;

		public UserController(IAccountsService accountsService)
		{
			_accountsService = accountsService;
		}

		/// <summary>
		/// Open Admin Users Page
		/// </summary>
		/// <returns>Admin Users View</returns>
		[HttpGet("All")]
		public async Task<IActionResult> GetAll()
		{
			var users = await _accountsService.GetAllAsync();
			return Ok(users);
		}

		/// <summary>
		/// Open Admin User Details Page
		/// </summary>
		/// <returns>Admin Users View</returns>
		/// <param name="id">Target user Id</param>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			var user = await _accountsService.GetAsync(id);
			if (user is null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		/// <summary>
		/// Add a new user
		/// </summary>
		/// <returns>Admins users View</returns> 
		/// <param name="register">Target register user model</param>
		[HttpPost(nameof(Add))]
		public async Task<IActionResult> Add([FromBody] AdminRegisterViewModel register)
		{
			var isModelValid = await _accountsService.IsRegisterValidAsync(ModelState, register);

			if (!isModelValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			await _accountsService.AddAsync(register);

			return Ok(new { Message = $"New user added successfully" });
		}

		/// <summary>
		/// Change password for user
		/// </summary>
		/// <returns>User Details Page</returns>
		/// <param name="changePassword">Target changePassword model</param>  
		[HttpPost(nameof(ChangePassword))]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePassword)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			var isSuccess = await _accountsService.ChangePasswordAsync(changePassword);
			if (isSuccess)
			{
				return Ok(new { Message = $"Password for user ({changePassword.UserId}) changes successfully" });
			}

			return NotFound(new { Message = $"User ({changePassword.UserId}) not found" });
		}

		/// <summary>
		/// Update target user
		/// </summary>
		/// <returns>Operation StatusCode<</returns>
		/// <param name="editUser">Target EditUser model</param>  
		[HttpPost(nameof(Update))]
		public async Task<IActionResult> Update([FromBody] AdminEditUserViewModel editUser)
		{
			var isModelValid = await _accountsService.IsEditUserValidAsync(ModelState, editUser);

			if (!isModelValid)
			{
				return BadRequest(new { Message = "Invalid input data", Errors = ModelState.GetErrors() });
			}

			var isSuccess = await _accountsService.UpdateInfoAsync(editUser);
			if (isSuccess)
			{
				return Ok(new { Message = $"Data for user ({editUser.Id}) updated successfully" });
			}

			return NotFound(new { Message = $"User ({editUser.Id}) not found" });
		}

		/// <summary>
		/// Delete user by Id
		/// </summary>
		/// <returns>Operation StatusCode<</returns>
		/// <param name="id">Target user Id</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var isSuccess = await _accountsService.DeleteAsync(id);

			if (isSuccess)
			{
				return Ok(new { Message = $"User ({id}) deleted successfully" });
			}
			return NotFound(new { Message = $"User ({id}) not found" });
		}
	}
}
