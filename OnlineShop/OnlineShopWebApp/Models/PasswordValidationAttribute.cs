using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShopWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
    public class PasswordValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PasswordPattern { get; } = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$";

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-passwordvalidation", ErrorMessage);
            context.Attributes.Add("data-val-passwordvalidation-pattern", PasswordPattern);
        }

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }

            var password = (string)value;

            var isValid = Regex.IsMatch(password, PasswordPattern);

            return isValid;
        }
    }
}
