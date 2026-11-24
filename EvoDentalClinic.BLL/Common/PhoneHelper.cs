using System.Text.RegularExpressions;

namespace EvoDentalClinic.BLL.Common;

public static class PhoneHelper
{
    public static string? Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var digits = Regex.Replace(input, @"[\s\-]", "");

        if (digits.StartsWith("+20")) digits = "0" + digits[3..];
        else if (digits.StartsWith("0020")) digits = "0" + digits[4..];

        return Regex.IsMatch(digits, @"^01[0125]\d{8}$") ? digits : null;
    }
}