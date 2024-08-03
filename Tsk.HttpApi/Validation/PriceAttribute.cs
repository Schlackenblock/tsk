using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Tsk.HttpApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PriceAttribute : ValidationAttribute
{
    public PriceAttribute()
        : base("The field {0} must be a valid price (greater than 0 and with 2 decimal places).")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not decimal decimalValue)
        {
            throw new Exception($"The {GetType().FullName} attribute was applied to a non-decimal member.");
        }

        return
            decimalValue > 0 &&
            decimalValue.Scale <= 2;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
    }
}
