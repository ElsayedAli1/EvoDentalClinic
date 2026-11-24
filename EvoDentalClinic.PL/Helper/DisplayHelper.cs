using System.Globalization;
using EvoDentalClinic.DAL.Enums;

namespace EvoDentalClinic.PL.Helpers;

public static class DisplayHelper
{
    public static string ToArabicTime(this TimeOnly time)
    {
        var hour12 = time.Hour % 12 == 0 ? 12 : time.Hour % 12;
        var suffix = time.Hour < 12 ? "ص" : "م";
        return $"{hour12}:{time.Minute:D2} {suffix}";
    }

    public static string ToArabicDay(this DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Saturday => "السبت",
        DayOfWeek.Sunday => "الأحد",
        DayOfWeek.Monday => "الاثنين",
        DayOfWeek.Tuesday => "الثلاثاء",
        DayOfWeek.Wednesday => "الأربعاء",
        DayOfWeek.Thursday => "الخميس",
        _ => "الجمعة"
    };

    public static string ToArabicDate(this DateOnly date)
        => $"{date.ToArabicDay()} {date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

    // الصيغة اللي المتصفح (input type=date) والـ URL بيفهموها
    public static string ToIso(this DateOnly date)
        => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string ToArabic(this AppointmentStatus status) => status switch
    {
        AppointmentStatus.Booked => "محجوز",
        AppointmentStatus.Completed => "تم الكشف",
        AppointmentStatus.Cancelled => "ملغي",
        AppointmentStatus.NoShow => "لم يحضر",
        _ => ""
    };

    public static string ToBadgeClass(this AppointmentStatus status) => status switch
    {
        AppointmentStatus.Booked => "bg-primary",
        AppointmentStatus.Completed => "bg-success",
        AppointmentStatus.Cancelled => "bg-secondary",
        AppointmentStatus.NoShow => "bg-warning text-dark",
        _ => "bg-light text-dark"
    };

    public static string ToArabic(this Gender gender)
    => gender == Gender.Male ? "ذكر" : "أنثى";

    public static string ToArabic(this MaritalStatus status)
        => status == MaritalStatus.Married ? "متزوج" : "أعزب";

    public static string ToMoney(this decimal amount)
        => amount.ToString("#,##0.##", CultureInfo.InvariantCulture);
    private static readonly TimeZoneInfo CairoZone = FindCairoZone();

    private static TimeZoneInfo FindCairoZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); }   // Windows
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); } // Linux
    }

    public static string ToCairoDateTime(this DateTime utc)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc), CairoZone);

        return $"{local.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} {TimeOnly.FromDateTime(local).ToArabicTime()}";
    }
    public static string ToArabic(this DayOfWeek day) => day switch
    {
        DayOfWeek.Saturday => "السبت",
        DayOfWeek.Sunday => "الأحد",
        DayOfWeek.Monday => "الاثنين",
        DayOfWeek.Tuesday => "الثلاثاء",
        DayOfWeek.Wednesday => "الأربعاء",
        DayOfWeek.Thursday => "الخميس",
        _ => "الجمعة"
    };

    // "من السبت إلى الخميس" لو الأيام ورا بعض، وإلا بتتعدّد بفواصل
    public static string ToArabicRange(this IEnumerable<DayOfWeek> days)
    {
        // الأسبوع عندنا بيبدأ من السبت
        static int Index(DayOfWeek d) => ((int)d + 1) % 7;

        var ordered = days.Distinct().OrderBy(Index).ToList();

        if (ordered.Count == 0) return "";
        if (ordered.Count == 1) return ordered[0].ToArabic();

        var consecutive = true;
        for (var i = 1; i < ordered.Count; i++)
        {
            if (Index(ordered[i]) - Index(ordered[i - 1]) != 1)
            {
                consecutive = false;
                break;
            }
        }

        return consecutive
            ? $"من {ordered[0].ToArabic()} إلى {ordered[^1].ToArabic()}"
            : string.Join("، ", ordered.Select(d => d.ToArabic()));
    }
    public static string ToArabic(this ReviewStatus status) => status switch
    {
        ReviewStatus.Pending => "قيد المراجعة",
        ReviewStatus.Approved => "منشور",
        ReviewStatus.Rejected => "مرفوض",
        _ => ""
    };

    public static string ToBadgeClass(this ReviewStatus status) => status switch
    {
        ReviewStatus.Pending => "bg-warning text-dark",
        ReviewStatus.Approved => "bg-success",
        ReviewStatus.Rejected => "bg-secondary",
        _ => "bg-light text-dark"
    };

    public static string ToCairoDate(this DateTime utc)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc), CairoZone);

        return local.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }
}