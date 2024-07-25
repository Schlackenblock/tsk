using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Tsk.HttpApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PriceAttribute : ValidationAttribute
{
    private const string errorMessageTemplate = "The field {0} must be a valid price.";

    private readonly GreaterThanAttribute greaterThanAttribute;
    private readonly RegularExpressionAttribute regularExpressionAttribute;

    public PriceAttribute()
        : base(errorMessageTemplate)
    {
        greaterThanAttribute = new GreaterThanAttribute(0);
        regularExpressionAttribute = new RegularExpressionAttribute(@"^\d+\.\d{2}$");
    }

    public override bool IsValid(object? value)
    {
        return
            value is decimal &&
            greaterThanAttribute.IsValid(value) &&
            regularExpressionAttribute.IsValid(value);
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
    }
}
