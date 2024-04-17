using System.ComponentModel.DataAnnotations;

namespace Tsk.HttpApi.Validation;

public class GreaterThanAttribute : RangeAttribute
{
    public bool IsExclusive
    {
        get => MinimumIsExclusive;
        set => MinimumIsExclusive = value;
    }

    public GreaterThanAttribute(double min)
        : base(min, double.PositiveInfinity)
    {
        IsExclusive = true;
        ErrorMessage = "The field {0} must be greater than {1}.";
    }
}
