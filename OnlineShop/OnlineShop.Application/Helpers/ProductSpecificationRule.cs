﻿using System.Text.RegularExpressions;

namespace OnlineShop.Application.Helpers
{
    public class ProductSpecificationRule
    {
        public string Name { get; set; }
        public string? ValidationPattern { get; set; }
        public string? ErrorMessage { get; set; }

        public ProductSpecificationRule(string name, string? validationPattern = null, string? errorMessage = null)
        {
            Name = name;
            ValidationPattern = validationPattern;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Checks the passed value for compliance with the rule.
        /// </summary>
        /// <returns>true if value is valid; otherwise false</returns>
        /// <param name="value">Target value</param>
        /// <param name="error">Validation error</param>
        public bool IsValid(string? value, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(value))
            {
                error = ErrorMessage ?? $"Характеристика '{Name}' обязательна.";
                return false;
            }

            if (!string.IsNullOrEmpty(ValidationPattern) && !Regex.IsMatch(value, ValidationPattern))
            {
                error = ErrorMessage ?? $"Характеристика '{Name}' не соответствует шаблону.";
                return false;
            }

            return true;
        }
    }
}
