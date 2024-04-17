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

public class GreaterThanOrEqualToAttribute : RangeAttribute
{
    public GreaterThanOrEqualToAttribute(double min)
        : base(min, double.PositiveInfinity) =>
        ErrorMessage = "The field {0} must be greater than or equal to {1}.";
}
