using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;

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
                modelState.AddModelError(string.Empty, "Email и пароль не должны совпадать");
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
    }
}
