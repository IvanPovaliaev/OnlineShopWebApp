using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShopWebApp.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
AllowMultiple = false)]
    public class PhoneValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PhonePattern { get; } = @"^\+7\s?\(?\d{3}\)?\s?\d{3}(-|\s)?\d{2}(-|\s)?\d{2}$";

        public PhoneValidationAttribute()
        {
            ErrorMessage = "Номер должен соответствовать формату: +7 (XXX) XXX-XX-XX. Допускается отсутствие кода номера и" +
            " разделителей \"-\", наличие одного пробела между значащими цифрами. Например: +7 XXX XXX XX XX";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-phonevalidation", ErrorMessage);
            context.Attributes.Add("data-val-phonevalidation-pattern", PhonePattern);
        }

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }

            var password = (string)value;

            var isValid = Regex.IsMatch(password, PhonePattern);

            return isValid;
        }
    }
}
