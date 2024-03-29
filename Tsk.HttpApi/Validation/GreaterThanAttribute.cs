using System.ComponentModel.DataAnnotations;

namespace Tsk.HttpApi.Validation;

public class GreaterThanAttribute : RangeAttribute
{
    public GreaterThanAttribute(double exclusiveMin)
        : base(exclusiveMin, double.PositiveInfinity)
    {
        MinimumIsExclusive = true;
        ErrorMessage = "The field {0} must be greater than {1}.";
    }
}
