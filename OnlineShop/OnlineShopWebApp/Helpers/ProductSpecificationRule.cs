using System.Text.RegularExpressions;

namespace OnlineShopWebApp.Helpers
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
