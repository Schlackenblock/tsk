namespace Tsk.Tests;

public static class RandomExtensions
{
    public static double NextDouble(this Random random, double minValue, double maxValue, int precision)
    {
        var unadjustedDouble = random.NextDouble();

        var range = maxValue - minValue;
        var adjustedDouble = unadjustedDouble * range + minValue;

        return Math.Round(adjustedDouble, precision);
    }
}
