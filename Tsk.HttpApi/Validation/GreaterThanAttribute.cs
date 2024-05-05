using System.ComponentModel.DataAnnotations;

namespace Tsk.HttpApi.Validation;

public class GreaterThanAttribute : RangeAttribute
{
    public bool IsExclusive
    {
        get => MinimumIsExclusive;
        set
        {
            MinimumIsExclusive = value;
            ErrorMessage = value
                ? "The field {0} must be greater than {1}."
                : "The field {0} must be greater than or equal to {1}.";
        }
    }

    public GreaterThanAttribute(double min)
        : base(min, double.PositiveInfinity) =>
        IsExclusive = true;
}
