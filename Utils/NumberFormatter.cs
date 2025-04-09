namespace ranch_mayhem_engine.Utils;

public class NumberFormatter : IFormatProvider, ICustomFormatter
{
    public object? GetFormat(Type? formatType)
    {
        return formatType == typeof(ICustomFormatter) ? this : null;
    }

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (arg is not IFormattable)
        {
            if (format != null) return string.Format(format, arg);
        }

        var formattable = (IFormattable)arg!;

        if (format?.ToUpper() == "N")
        {
            if (arg is double or float)
            {
                var number = Convert.ToDouble(arg);
                return FormatNumber(number);
            }

            if (arg is int i)
            {
                return FormatNumber(i);
            }

            if (arg is long l)
            {
                return FormatNumber(l);
            }

            if (arg is decimal d)
            {
                return FormatNumber((double)d);
            }
        }

        return formattable!.ToString(format, formatProvider);
    }

    private static string FormatNumber(double number)
    {
        const double trillion = 1e12;
        const double billion = 1e9;
        const double million = 1e6;

        return number switch
        {
            >= trillion => $"{number / trillion:0.##}T",
            >= billion => $"{number / billion:0.##}B",
            >= million => $"{number / million:0.##}M",
            _ => number.ToString($"#,0")
        };
    }
}