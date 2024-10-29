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
        public List<UserViewModel> GetAll()
        {
            return _usersRepository.GetAll()
                                   .Select(_mapper.Map<UserViewModel>)
                                   .ToList();
        }

        /// <summary>
        /// Get user from repository by GUID
        /// </summary>
        /// <returns>UserViewModel; returns null if user not found</returns>
        /// <param name="id">Target user id (GUID)</param>
        public UserViewModel Get(Guid id)
        {
            var userDb = _usersRepository.Get(id);
            return _mapper.Map<UserViewModel>(userDb);
        }

        /// <summary>
        /// Add a new user to repository based on register info
        /// </summary>        
        /// <param name="register">Target register model</param>
        public void Add(RegisterViewModel register)
        {
            var roleId = GetRegisterRoleId(register);

            var user = new User
            {
                Email = register.Email,
                Password = _hashService.GenerateHash(register.Password),
                Name = register.Name,
                Phone = register.Phone,
                Role = _rolesService.Get(roleId)
            };

            _usersRepository.Add(user);
        }

        /// <summary>
        /// Change password for related user if user exist
        /// </summary>        
        /// <param name="changePassword">Target ChangePassword model</param>
        public void ChangePassword(ChangePasswordViewModel changePassword)
        {
            var userId = changePassword.UserId;
            var user = _usersRepository.Get(userId);

            if (user is null)
            {
                return;
            }

            user.Password = _hashService.GenerateHash(changePassword.Password);

            _usersRepository.Update(user);
        }

        /// <summary>
        /// Update info for related user if user exist
        /// </summary>        
        /// <param name="editUser">Target editUser model</param>
        public void UpdateInfo(AdminEditUserViewModel editUser)
        {
            var userId = editUser.UserId;
            var user = _usersRepository.Get(userId);

            if (user is null)
            {
                return;
            }

            var role = _rolesService.Get(editUser.RoleId);

            user.Email = editUser.Email;
            user.Phone = editUser.Phone;
            user.Name = editUser.Name;
            user.Role = role;

            _usersRepository.Update(user);
        }

        /// <summary>
        /// Delete user from repository by id
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        public void Delete(Guid id) => _usersRepository.Delete(id);

        /// <summary>
        /// Validates the user login model
        /// </summary>        
        /// <returns>true if login model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="login">Target login model</param>
        public bool IsLoginValid(ModelStateDictionary modelState, LoginViewModel login)
        {
            var user = _usersRepository.GetByEmail(login.Email);

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
        public bool IsRegisterValid(ModelStateDictionary modelState, RegisterViewModel register)
        {
            if (register.Email == register.Password)
            {
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать!");
            }

            if (IsEmailExist(register.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            if (register is AdminRegisterViewModel { RoleId: var roleId } && !IsRoleExist(roleId))
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
        public bool IsEditUserValid(ModelStateDictionary modelState, AdminEditUserViewModel editUser)
        {
            var repositoryUser = Get(editUser.UserId);

            if (repositoryUser.Email != editUser.Email & IsEmailExist(editUser.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            if (!IsRoleExist(editUser.RoleId))
            {
                modelState.AddModelError(string.Empty, "Роль не существует!");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Change all users role related to role Id to user Role.
        /// </summary>
        /// <param name="oldRoleId">Target old role Id (guid)</param>
        public void ChangeRolesToUser(Guid oldRoleId)
        {
            var userRoleId = _rolesService.GetAll()
                                          .FirstOrDefault(r => r.Name == Constants.UserRoleName)!
                                          .Id;

            _usersRepository.ChangeRolesToUser(oldRoleId, userRoleId);
        }

        /// <summary>
        /// Get MemoryStream for all users export to Excel 
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        public MemoryStream ExportAllToExcel()
        {
            var users = GetAll();
            return _excelService.ExportUsers(users);
        }

        /// <summary>
        /// Get a role for new user based on register model
        /// </summary>        
        /// <returns>Associated Role Id; Return Role User Id as default</returns>
        /// <param name="register">Target register model</param>
        private Guid GetRegisterRoleId(RegisterViewModel register)
        {
            return register switch
            {
                AdminRegisterViewModel adminRegister => adminRegister.RoleId,
                _ => _rolesService.GetAll()
                                  .First(r => r.Name == Constants.UserRoleName)
                                  .Id
            };
        }

        /// <summary>
        /// Checks if a user with the given address exists.
        /// </summary>        
        /// <returns>true if user with target email already exists; otherwise false</returns>
        /// <param name="email">Target email</param>
        private bool IsEmailExist(string email)
        {
            var users = _usersRepository.GetAll();

            return users.Any(users => users.Email == email);
        }

        /// <summary>
        /// Checks if a role with the given id exists.
        /// </summary>        
        /// <returns>true if exists; otherwise false</returns>
        /// <param name="roleId">Target role id (GUID)</param>
        private bool IsRoleExist(Guid roleId)
        {
            var role = _rolesService.Get(roleId);

            return role != null;
        }
    }
}
