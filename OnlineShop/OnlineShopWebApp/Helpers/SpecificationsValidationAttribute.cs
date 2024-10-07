using Microsoft.Extensions.DependencyInjection;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Helpers
{
    public class SpecificationsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var product = (Product)validationContext.ObjectInstance;

            if (product.Specifications == null || product.Specifications.Count == 0)
            {
                return new ValidationResult("Характеристики не могут отсутствовать");
            }

            var productService = validationContext.GetService<ProductsService>(); //Избавиться

            var rules = productService
                            .GetSpecificationsRules(product.Category)
                            .GetAll();

            var validationErrors = new List<string>();

            foreach (var rule in rules)
            {
                if (!product.Specifications.TryGetValue(rule.Name, out var specValue))
                {
                    validationErrors.Add(rule.ErrorMessage ?? $"Отсутствует необходимая характеристика: {rule.Name}");
                    continue;
                }

                if (!rule.IsValid(specValue, out var errorMessage))
                {
                    validationErrors.Add(errorMessage);
                }
            }

            if (validationErrors.Count != 0)
            {
                return new ValidationResult(string.Join("\n", validationErrors));
            }

            return ValidationResult.Success;
        }
    }
}
