namespace EvoDentalClinic.BLL.Common;

public class ClinicClock : IClinicClock
{
    private static readonly TimeZoneInfo CairoZone = FindCairoZone();

    private static DateTime Now
        => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CairoZone);

    public DateOnly Today => DateOnly.FromDateTime(Now);
    public TimeOnly TimeNow => TimeOnly.FromDateTime(Now);

    private static TimeZoneInfo FindCairoZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); } // Windows
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); } // Linux
    }
    public DateOnly ToClinicDate(DateTime utc)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc), CairoZone);

        return DateOnly.FromDateTime(local);
    }
}