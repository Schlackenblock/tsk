using System.ComponentModel.DataAnnotations;

namespace Tsk.HttpApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PriceAttribute : ValidationAttribute
{
    private const string errorMessageTemplate = "The field {0} must be a valid price.";

    private readonly RegularExpressionAttribute regularExpressionAttribute;
    private readonly GreaterThanAttribute greaterThanAttribute;

    public PriceAttribute()
        : base(errorMessageTemplate)
    {
        regularExpressionAttribute = new RegularExpressionAttribute(@"^\d+\.\d{2}$");
        greaterThanAttribute = new GreaterThanAttribute(0);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!regularExpressionAttribute.IsValid(value))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        if (!greaterThanAttribute.IsValid(value))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
}
