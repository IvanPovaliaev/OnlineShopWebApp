using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Models.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class AccountsService
    {
        private readonly IMapper _mapper;
        private readonly RolesService _rolesService;
        private readonly IExcelService _excelService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountsService(IMapper mapper, RolesService rolesService, IExcelService excelService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _mapper = mapper;
            _rolesService = rolesService;
            _excelService = excelService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all users from repository
        /// </summary>
        /// <returns>List of all UserViewModel from repository</returns>
        public virtual async Task<List<UserViewModel>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersViewModels = new List<UserViewModel>(users.Count);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userVM = _mapper.Map<UserViewModel>(user);
                userVM.RoleName = roles.FirstOrDefault()!;
                usersViewModels.Add(userVM);
            }

            return usersViewModels;
        }

        /// <summary>
        /// Get user from repository by GUID
        /// </summary>
        /// <returns>UserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id</param>
        public virtual async Task<UserViewModel> GetAsync(string id)
        {
            var userDb = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(userDb!);
            var userVM = _mapper.Map<UserViewModel>(userDb);
            userVM.RoleName = roles.First();

            return userVM;
        }

        /// <summary>
        /// Add a new user to repository based on register info
        /// </summary>        
        /// <param name="register">Target register model</param>
        public virtual async Task AddAsync(RegisterViewModel register)
        {
            var user = new User
            {
                Email = register.Email,
                UserName = register.Email,
                FullName = register.Name,
                PhoneNumber = register.PhoneNumber
            };

            await _userManager.CreateAsync(user, register.Password);

            var roleName = await GetRegisterRoleNameAsync(register);
            await _userManager.AddToRoleAsync(user, roleName);

            if (register is not AdminRegisterViewModel)
            {
                await _signInManager.SignInAsync(user, false);
            }
        }

        /// <summary>
        /// Change password for related user if user exist
        /// </summary>        
        /// <param name="changePassword">Target ChangePassword model</param>
        public virtual async Task ChangePasswordAsync(ChangePasswordViewModel changePassword)
        {
            var userId = changePassword.UserId;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return;
            }

            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, changePassword.Password);
            user.PasswordHash = newPasswordHash;

            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Update info for related user if user exist
        /// </summary>        
        /// <param name="editUser">Target editUser model</param>
        public virtual async Task UpdateInfoAsync(AdminEditUserViewModel editUser)
        {
            var userId = editUser.UserId;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return;
            }

            user.Email = editUser.Email;
            user.PhoneNumber = editUser.Phone;
            user.FullName = editUser.Name;

            await _userManager.UpdateAsync(user);

            var role = await _rolesService.GetAsync(editUser.RoleId);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.AddToRoleAsync(user, role.Name);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
        }

        /// <summary>
        /// Delete user from repository by id. Admin can't be deleted
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        public virtual async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var isAdmin = await _userManager.IsInRoleAsync(user!, Constants.AdminRoleName);

            if (!isAdmin)
            {
                await _userManager.DeleteAsync(user!);
            }
        }

        /// <summary>
        /// Validates the user login model
        /// </summary>        
        /// <returns>true if login model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="login">Target login model</param>
        public virtual async Task<bool> IsLoginValidAsync(ModelStateDictionary modelState, LoginViewModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.KeepMeLogged, false);

            if (!result.Succeeded)
            {
                modelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Validates the registration model
        /// </summary>        
        /// <returns>true if registration model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="register">Target register model</param>
        public virtual async Task<bool> IsRegisterValidAsync(ModelStateDictionary modelState, RegisterViewModel register)
        {
            if (register.Email == register.Password)
            {
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать!");
            }

            if (await IsEmailExistAsync(register.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            if (register is AdminRegisterViewModel { RoleId: var roleId } && !await IsRoleExistAsync(roleId))
            {
                modelState.AddModelError(string.Empty, "Роль не существует!");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Validates the user edit model
        /// </summary>        
        /// <returns>true if edit model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="editUser">Target edit model</param>
        public virtual async Task<bool> IsEditUserValidAsync(ModelStateDictionary modelState, AdminEditUserViewModel editUser)
        {
            var repositoryUser = await GetAsync(editUser.UserId);

            if (repositoryUser.Email != editUser.Email & await IsEmailExistAsync(editUser.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            var isRoleExist = await IsRoleExistAsync(editUser.RoleId);

            if (!isRoleExist)
            {
                modelState.AddModelError(string.Empty, "Роль не существует!");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Logout user
        /// </summary>
        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        /// <summary>
        /// Change all users role related to role Id to user Role.
        /// </summary>
        /// <param name="oldRoleId">Target old role Id (guid)</param>
        public async Task ChangeRolesToUserAsync(string oldRoleId)
        {
            var oldRole = (await _rolesService.GetAllAsync())
                                                 .FirstOrDefault(r => r.Id == oldRoleId)!
                                                 .Name;

            var users = await _userManager.GetUsersInRoleAsync(oldRole);
        }

        /// <summary>
        /// Get MemoryStream for all users export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        public virtual async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var users = await GetAllAsync();
            return _excelService.ExportUsers(users);
        }

        /// <summary>
        /// Get a role name for new user based on register model
        /// </summary>        
        /// <returns>Associated Role name; Return User role name as default</returns>
        /// <param name="register">Target register model</param>
        private async Task<string> GetRegisterRoleNameAsync(RegisterViewModel register)
        {
            if (register is AdminRegisterViewModel)
            {
                var adminRegister = register as AdminRegisterViewModel;
                var role = await _rolesService.GetAsync(adminRegister!.RoleId);
                return role?.Name ?? Constants.UserRoleName;
            }

            return Constants.UserRoleName;
        }

        /// <summary>
        /// Checks if a user with the given address exists.
        /// </summary>        
        /// <returns>true if user with target email already exists; otherwise false</returns>
        /// <param name="email">Target email</param>
        private async Task<bool> IsEmailExistAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user is not null;
        }

        /// <summary>
        /// Checks if a role with the given id exists.
        /// </summary>        
        /// <returns>true if exists; otherwise false</returns>
        /// <param name="roleId">Target role id (GUID)</param>
        private async Task<bool> IsRoleExistAsync(string roleId)
        {
            var role = await _rolesService.GetAsync(roleId);

            return role != null;
        }
    }
}
