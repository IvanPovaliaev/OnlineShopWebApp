using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineShop.Application.Helpers;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Application.Models.DTO;
using OnlineShop.Application.Models.Options;
using OnlineShop.Domain;
using OnlineShop.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IMapper _mapper;
        private readonly IRolesService _rolesService;
        private readonly IExcelService _excelService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ImagesProvider _imageProvider;
        private readonly string _usersAvatarsStoragePath;

        public AccountsService(IMapper mapper, IRolesService rolesService, IExcelService excelService, SignInManager<User> signInManager, UserManager<User> userManager, IOptions<ImagesStorage> imagesStorage, ImagesProvider imagesProvider)
        {
            _mapper = mapper;
            _rolesService = rolesService;
            _excelService = excelService;
            _signInManager = signInManager;
            _userManager = userManager;

            _usersAvatarsStoragePath = imagesStorage.Value.AvatarsPath;
            _imageProvider = imagesProvider;
        }

        public async Task<List<UserViewModel>> GetAllAsync()
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

        public async Task<UserViewModel> GetAsync(string id)
        {
            var userDb = await _userManager.FindByIdAsync(id);

            if (userDb is null)
            {
                return null!;
            }

            var roles = await _userManager.GetRolesAsync(userDb!);

            var userVM = _mapper.Map<UserViewModel>(userDb);
            userVM.RoleName = roles.FirstOrDefault()!;

            return userVM;
        }

        public async Task<EditUserViewModel> GetEditViewModelAsync(string id)
        {
            var userDb = await _userManager.FindByIdAsync(id);
            if (userDb is null)
            {
                return null!;
            }

            return _mapper.Map<EditUserViewModel>(userDb);
        }

        public async Task AddAsync(RegisterViewModel register)
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

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel changePassword)
        {
            var userId = changePassword.UserId;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, changePassword.Password);
            user.PasswordHash = newPasswordHash;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateInfoAsync(EditUserViewModel editUser)
        {
            var userId = editUser.Id;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            user.Email = editUser.Email;
            user.UserName = editUser.Email;
            user.PhoneNumber = editUser.PhoneNumber;
            user.FullName = editUser.FullName;
            var avatarUrl = await _imageProvider.SaveAsync(editUser.UploadedImage, _usersAvatarsStoragePath);

            if (avatarUrl != null)
            {
                user.AvatarUrl = avatarUrl;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }

            if (editUser is AdminEditUserViewModel adminEditUser)
            {
                await ChangeRoleAsync(user, adminEditUser.RoleName);
                return true;
            }

            await _signInManager.RefreshSignInAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var isAdmin = await _userManager.IsInRoleAsync(user!, Constants.AdminRoleName);

            if (isAdmin)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user!);
            return result.Succeeded;
        }

        public async Task<bool> IsLoginValidAsync(ModelStateDictionary modelState, LoginDTO login, bool keepMeLogged = false)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, keepMeLogged, false);

            if (!result.Succeeded)
            {
                modelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }

            return modelState.IsValid;
        }

        public async Task<bool> IsRegisterValidAsync(ModelStateDictionary modelState, RegisterViewModel register)
        {
            if (register.Email == register.Password)
            {
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать!");
            }

            if (await IsEmailExistAsync(register.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            if (register is AdminRegisterViewModel { RoleName: var roleName } && !await IsRoleExistAsync(roleName))
            {
                modelState.AddModelError(string.Empty, "Роль не существует!");
            }

            return modelState.IsValid;
        }

        public async Task<bool> IsEditUserValidAsync(ModelStateDictionary modelState, EditUserViewModel editUser)
        {
            var repositoryUser = await GetAsync(editUser.Id);

            if (repositoryUser.Email != editUser.Email & await IsEmailExistAsync(editUser.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            if (editUser is AdminEditUserViewModel adminEditUser)
            {
                var isRoleExist = await IsRoleExistAsync(adminEditUser.RoleName);

                if (!isRoleExist)
                {
                    modelState.AddModelError(string.Empty, "Роль не существует!");
                }
            }

            return modelState.IsValid;
        }

        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        public async Task ChangeRolesToUserAsync(string oldRoleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(oldRoleName);

            foreach (var user in users)
            {
                await _userManager.RemoveFromRoleAsync(user, oldRoleName);
                await _userManager.AddToRoleAsync(user, Constants.UserRoleName);
            }
        }

        public async Task<MemoryStream> ExportAllToExcelAsync()
        {
            var users = await GetAllAsync();
            return _excelService.ExportUsers(users);
        }

        public async Task<bool> IsForgotPasswordValidAsync(ModelStateDictionary modelState, ForgotPasswordViewModel model)
        {
            if (!await IsEmailExistAsync(model.Email))
            {
                modelState.AddModelError(string.Empty, "Пользователь с таким Email не найден");
            }

            return modelState.IsValid;
        }

        public async Task<bool> IsResetPasswordValidAsync(ModelStateDictionary modelState, ResetPasswordViewModel model)
        {
            if (!await IsEmailExistAsync(model.Email))
            {
                modelState.AddModelError(string.Empty, "Пользователь с таким Email не найден");
            }

            if (model.Email == model.Password)
            {
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать!");
            }

            return modelState.IsValid;
        }

        public async Task<string> GetPasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await _userManager.GeneratePasswordResetTokenAsync(user!);
        }

        public async Task<bool> TryResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            return result.Succeeded;
        }

        public async Task<bool> IsUserExistAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user is not null;
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
                var role = await _rolesService.GetAsync(adminRegister!.RoleName);
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
        /// Checks if a role with the given name exists.
        /// </summary>        
        /// <returns>true if exists; otherwise false</returns>
        /// <param name="roleName">Target role name</param>
        private async Task<bool> IsRoleExistAsync(string roleName)
        {
            var role = await _rolesService.GetAsync(roleName);

            return role is not null;
        }

        /// <summary>
        /// Change role for related user to role with given name
        /// </summary>
        /// <param name="user">Target user</param>
        /// <param name="newRoleName">Target new role name</param>
        private async Task ChangeRoleAsync(User user, string newRoleName)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles is null || userRoles.Contains(newRoleName))
            {
                return;
            }

            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, newRoleName);
        }
    }
}
