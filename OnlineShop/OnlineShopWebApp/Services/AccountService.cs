using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Services
{
    public class AccountService
    {
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
                return false;
            }

            return true;
        }
    }
}
