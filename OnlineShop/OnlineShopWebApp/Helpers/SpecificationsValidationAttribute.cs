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
            var product = (ProductViewModel)validationContext.ObjectInstance;

            if (!IsSpecificationsExists(product))
            {
                return new ValidationResult("Характеристики не могут отсутствовать");
            }

            var productService = validationContext.GetService<ProductsService>();

            var rules = productService
                            .GetSpecificationsRules(product.Category)
                            .GetAll();

            if (!IsRulesValid(product, rules, out var validationErrors))
            {
                return new ValidationResult(string.Join("\n", validationErrors));
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Checks whether the product has specifications.
        /// </summary>
        /// <returns>true if specifications exists; otherwise false</returns>
        /// <param name="product">Target product</param>
        private bool IsSpecificationsExists(ProductViewModel product)
        {
            if (product?.Specifications == null || product?.Specifications.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks all specifications of the target product for compliance with related rules.
        /// </summary>
        /// <returns>true if all specifications are valid; otherwise return false</returns>
        /// <param name="product">Target product</param>
        /// <param name="rules">Collection of related rules</param>
        /// <param name="validationErrors">List of validation errors</param>
        private bool IsRulesValid(ProductViewModel product, IEnumerable<ProductSpecificationRule> rules, out List<string> validationErrors)
        {
            validationErrors = [];

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

            return validationErrors.Count == 0;
        }
    }
}
