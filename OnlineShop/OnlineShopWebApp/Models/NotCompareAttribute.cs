using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

public class NotCompareAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string _comparisonProperty;

    public NotCompareAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = value;
        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
        {
            throw new ArgumentException($"Property with name '{_comparisonProperty}' not found");
        }

        var comparisonValue = property.GetValue(validationContext.ObjectInstance);

        // Проверка на неравенство
        if (Equals(currentValue, comparisonValue))
        {
            return new ValidationResult(ErrorMessage ?? "Fields must not be equal");
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-notcompare", ErrorMessage ?? "Fields must not be equal");
        context.Attributes.Add("data-val-notcompare-other", _comparisonProperty);
    }
}