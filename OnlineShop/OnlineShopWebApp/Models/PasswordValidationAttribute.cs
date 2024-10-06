using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShopWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
    public class PasswordValidationAttribute : ValidationAttribute
    {
        private readonly string _passwordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$";

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return false;
            }

            var password = value as string;

            var result = Regex.IsMatch(password, _passwordPattern);

            return result;
        }
    }
}
