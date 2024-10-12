using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class AccountsService
    {
        private readonly IUsersRepository _usersRepository;

        public AccountsService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Validates the user registration model
        /// </summary>        
        /// <returns>true if registration model is valid; otherwise false</returns>
        /// <param name="modelState">Current model state</param>
        /// /// <param name="register">Target register model</param>
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
                Password = register.Password,
                Name = register.Name,
                Phone = register.Phone
            };

            _usersRepository.Add(user);
        }

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
