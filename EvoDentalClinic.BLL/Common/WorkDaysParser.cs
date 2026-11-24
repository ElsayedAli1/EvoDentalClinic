namespace EvoDentalClinic.BLL.Common;

public static class WorkDaysParser
{
    public static List<DayOfWeek> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<DayOfWeek>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(d => Enum.TryParse<DayOfWeek>(d, ignoreCase: true, out var parsed) ? (DayOfWeek?)parsed : null)
            .Where(d => d is not null)
            .Select(d => d!.Value)
            .Distinct()
            .ToList();
    }
    public static string Format(IEnumerable<DayOfWeek> days)
    => string.Join(",", days
        .Distinct()
        .OrderBy(d => ((int)d + 1) % 7)   // الأسبوع بيبدأ من السبت
        .Select(d => d.ToString()));
}