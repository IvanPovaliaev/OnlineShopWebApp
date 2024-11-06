using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Helpers;
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

        public AccountsService(IUsersRepository usersRepository, IMapper mapper, RolesService rolesService, HashService hashService, IExcelService excelService)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _rolesService = rolesService;
            _hashService = hashService;
            _excelService = excelService;
        }

        /// <summary>
        /// Get all users from repository
        /// </summary>
        /// <returns>List of all UserViewModel from repository</returns>
        public async Task<List<UserViewModel>> GetAllAsync()
        {
            var users = await _usersRepository.GetAllAsync();
            return users.Select(_mapper.Map<UserViewModel>)
                        .ToList();
        }

        /// <summary>
        /// Get user from repository by GUID
        /// </summary>
        /// <returns>UserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id (GUID)</param>
        public async Task<UserViewModel> GetAsync(Guid id)
        {
            var userDb = await _usersRepository.GetAsync(id);
            return _mapper.Map<UserViewModel>(userDb);
        }

        /// <summary>
        /// Add a new user to repository based on register info
        /// </summary>        
        /// <param name="register">Target register model</param>
        public virtual async Task AddAsync(RegisterViewModel register)
        {
            var roleId = await GetRegisterRoleIdAsync(register);

            var user = new User
            {
                Email = register.Email,
                Password = _hashService.GenerateHash(register.Password),
                Name = register.Name,
                Phone = register.Phone,
                Role = await _rolesService.GetAsync(roleId)
            };

            await _usersRepository.AddAsync(user);
        }

        /// <summary>
        /// Change password for related user if user exist
        /// </summary>        
        /// <param name="changePassword">Target ChangePassword model</param>
        public async Task ChangePasswordAsync(ChangePasswordViewModel changePassword)
        {
            var userId = changePassword.UserId;
            var user = await _usersRepository.GetAsync(changePassword.UserId);

            if (user is null)
            {
                return;
            }

            user.Password = _hashService.GenerateHash(changePassword.Password);

            await _usersRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Update info for related user if user exist
        /// </summary>        
        /// <param name="editUser">Target editUser model</param>
        public async Task UpdateInfoAsync(AdminEditUserViewModel editUser)
        {
            var userId = editUser.UserId;
            var user = await _usersRepository.GetAsync(editUser.UserId);

            if (user is null)
            {
                return;
            }

            var role = await _rolesService.GetAsync(editUser.RoleId);

            user.Email = editUser.Email;
            user.Phone = editUser.Phone;
            user.Name = editUser.Name;
            user.Role = role;

            await _usersRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Delete user from repository by id
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        public async Task DeleteAsync(Guid id) => await _usersRepository.DeleteAsync(id);

        /// <summary>
        /// Validates the user login model
        /// </summary>        
        /// <returns>true if login model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="login">Target login model</param>
        public virtual async Task<bool> IsLoginValidAsync(ModelStateDictionary modelState, LoginViewModel login)
        {
            var user = await _usersRepository.GetByEmailAsync(login.Email);

            if (user is null)
            {
                modelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return modelState.IsValid;
            }

            var isPasswordsEquals = _hashService.IsEquals(login.Password, user.Password);

            if (!isPasswordsEquals)
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
        public async Task<bool> IsEditUserValidAsync(ModelStateDictionary modelState, AdminEditUserViewModel editUser)
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
        public async Task<MemoryStream> ExportAllToExcelAsync()
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
            var users = await _usersRepository.GetAllAsync();

            return users.Any(users => users.Email == email);
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
