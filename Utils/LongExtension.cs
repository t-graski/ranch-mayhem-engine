using System.Text;

namespace ranch_mayhem_engine.Utils;

public static class LongExtension
{
    private static readonly Dictionary<int, string> NumberRoman = new()
    {
        { 1000, "M" },
        { 900, "CM" },
        { 500, "D" },
        { 400, "CD" },
        { 100, "C" },
        { 90, "XC" },
        { 50, "L" },
        { 40, "XL" },
        { 10, "X" },
        { 9, "IX" },
        { 5, "V" },
        { 4, "IV" },
        { 1, "I" }
    };


    public static string ToRoman(this long number)
    {
        var roman = new StringBuilder();

        foreach (var item in NumberRoman)
        {
            while (number >= item.Key)
            {
                roman.Append(item.Value);
                number -= item.Key;
            }
        }

        return roman.ToString();
    }
}