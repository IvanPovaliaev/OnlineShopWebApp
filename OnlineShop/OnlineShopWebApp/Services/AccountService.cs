using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Services
{
    public class AccountService
    {
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
