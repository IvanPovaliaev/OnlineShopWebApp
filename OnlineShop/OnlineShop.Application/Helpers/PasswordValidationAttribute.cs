using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShop.Application.Helpers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
    public class PasswordValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PasswordPattern { get; } = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$";

        public PasswordValidationAttribute()
        {
            ErrorMessage = $"Пароль должен:\n" +
            $"Cодержать как минимум одну заглавную букву;\n" +
            $"Cодержать как минимум одну строчную букву;\n" +
            $"Cодержать как минимум одну цифру;\n" +
            $"Cодержать как минимум один из специальных символов \"#?!@$%^&*-\";\n" +
            $"Быть длиной от 8 до 20 символов.";
        }

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
