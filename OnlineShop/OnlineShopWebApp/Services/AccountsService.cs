using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class AccountsService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly HashService _hashService;

        public AccountsService(IUsersRepository usersRepository, HashService hashService)
        {
            _usersRepository = usersRepository;
            _hashService = hashService;
        }

        /// <summary>
        /// Get all users from repository
        /// </summary>
        /// <returns>List of all users from repository</returns>
        public List<User> GetAll() => _usersRepository.GetAll();

        /// <summary>
        /// Get user from repository by GUID
        /// </summary>
        /// <returns>Product; returns null if product not found</returns>
        /// <param name="id">Target user id (GUID)</param>
        public User Get(Guid id) => _usersRepository.Get(id);

        /// <summary>
        /// Validates the user login model
        /// </summary>        
        /// <returns>true if login model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="login">Target login model</param>
        public bool IsLoginValid(ModelStateDictionary modelState, Login login)
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
        /// Validates the user registration model
        /// </summary>        
        /// <returns>true if registration model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// <param name="register">Target register model</param>
        public bool IsRegisterValid(ModelStateDictionary modelState, Register register)
        {
            if (register.Email == register.Password)
            {
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать!");
            }

            if (IsEmailExist(register.Email))
            {
                modelState.AddModelError(string.Empty, "Email уже зарегистрирован!");
            }

            return modelState.IsValid;
        }

        /// <summary>
        /// Add a new user to repository based on register info
        /// </summary>        
        /// <param name="register">Target register model</param>
        public void Add(Register register)
        {
            var user = new User
            {
                Email = register.Email,
                Password = _hashService.GenerateHash(register.Password),
                Name = register.Name,
                Phone = register.Phone
            };

            _usersRepository.Add(user);
        }

        /// <summary>
        /// Change password for related user if user exist
        /// </summary>        
        /// <param name="changePassword">Target ChangePassword model</param>
        public void ChangePassword(ChangePassword changePassword)
        {
            var userId = changePassword.UserId;
            var user = Get(userId);

            if (user is null)
            {
                return;
            }

            user.Password = _hashService.GenerateHash(changePassword.Password);

            _usersRepository.Update(user);
        }

        /// <summary>
        /// Delete user from repository by id
        /// </summary>
        /// <param name="id">Target user id (GUID)</param>
        public void Delete(Guid id) => _usersRepository.Delete(id);

        /// <summary>
        /// Checks if a user with the given address exists.
        /// </summary>        
        /// <returns>true if user with target email already exists; otherwise false</returns>
        /// <param name="email">Target email</param>
        private bool IsEmailExist(string email)
        {
            var users = _usersRepository.GetAll();

            if (users.Any(users => users.Email == email))
            {
                return true;
            }

            return false;
        }
    }
}
