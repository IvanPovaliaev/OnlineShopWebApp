using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class AccountsService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly RolesService _rolesService;
        private readonly HashService _hashService;
        private readonly IExcelService _excelService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountsService(IUsersRepository usersRepository, IMapper mapper, RolesService rolesService, HashService hashService, IExcelService excelService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _rolesService = rolesService;
            _hashService = hashService;
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
            return users.Select(_mapper.Map<UserViewModel>)
                        .ToList();
        }

        /// <summary>
        /// Get user from repository by GUID
        /// </summary>
        /// <returns>UserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id</param>
        public virtual async Task<UserViewModel> GetAsync(string id)
        {
            var userDb = await _userManager.FindByIdAsync(id);
            return _mapper.Map<UserViewModel>(userDb);
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
                PhoneNumber = register.Phone
            };

            await _userManager.CreateAsync(user, register.Password);
            await _signInManager.SignInAsync(user, false);
            await _userManager.AddToRoleAsync(user, Constants.UserRoleName);
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

            user.PasswordHash = _hashService.GenerateHash(changePassword.Password);

            await _usersRepository.UpdateAsync(user);
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

            var role = await _rolesService.GetAsync(editUser.RoleId);

            user.Email = editUser.Email;
            user.PhoneNumber = editUser.Phone;
            user.FullName = editUser.Name;
            //user.Role = role;

            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Delete user from repository by id
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        public virtual async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user!);
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

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Change all users role related to role Id to user Role.
        /// </summary>
        /// <param name="oldRoleId">Target old role Id (guid)</param>
        public async Task ChangeRolesToUserAsync(Guid oldRoleId)
        {
            var userRoleId = (await _rolesService.GetAllAsync())
                                                 .FirstOrDefault(r => r.Name == Constants.UserRoleName)!
                                                 .Id;

            await _usersRepository.ChangeRolesToUserAsync(oldRoleId, userRoleId);
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
        /// Get a role for new user based on register model
        /// </summary>        
        /// <returns>Associated Role Id; Return Role User Id as default</returns>
        /// <param name="register">Target register model</param>
        private async Task<Guid> GetRegisterRoleIdAsync(RegisterViewModel register)
        {
            if (register is AdminRegisterViewModel)
            {
                var adminRegister = register as AdminRegisterViewModel;
                return adminRegister!.RoleId;
            }

            var roles = await _rolesService.GetAllAsync();
            return roles.First(r => r.Name == Constants.UserRoleName)
                        .Id;
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
        private async Task<bool> IsRoleExistAsync(Guid roleId)
        {
            var role = await _rolesService.GetAsync(roleId);

            return role != null;
        }
    }
}
