namespace EvoDentalClinic.PL.Config;

public class ClinicInfoOptions
{
    public string Name { get; set; } = "";
    public string EnglishName { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public string DoctorTitle { get; set; } = "";
    public string Phone { get; set; } = "";
    public string WhatsApp { get; set; } = "";
    public string Address { get; set; } = "";
    public string? MapUrl { get; set; }
    //public List<string> Services { get; set; } = new();
    public string? FacebookUrl { get; set; }
    public List<ServiceItem> Services { get; set; } = new();
    // الناحية اللي فيها صورة "قبل" في صور الحالات: right أو left
    public string CasesBeforeSide { get; set; } = "right";

    // رابط الخريطة المدمجة (الـ src بتاع iframe من Google Maps)
    public string? MapEmbedUrl { get; set; }

    // بنقبل روابط Google Maps المدمجة بس، عشان محدش يحط رابط غريب بالغلط
    public string? MapEmbedSrc =>
        !string.IsNullOrWhiteSpace(MapEmbedUrl) &&
        MapEmbedUrl.StartsWith("https://www.google.com/maps/embed", StringComparison.OrdinalIgnoreCase)
            ? MapEmbedUrl
            : null;

    // رابط واتساب برسالة جاهزة
    public string WhatsAppBookingLink
        => $"{WhatsAppLink}?text={Uri.EscapeDataString("السلام عليكم، عايز أحجز ميعاد")}";
    // واتساب عايز الرقم بكود الدولة من غير الصفر: 01515981450 → 201515981450
    public string WhatsAppLink
    {
        get
        {
            var digits = new string(WhatsApp.Where(char.IsDigit).ToArray());
            if (digits.StartsWith('0')) digits = "20" + digits[1..];
            return $"https://wa.me/{digits}";
        }
    }
    public class ServiceItem
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
    }
    public string MapLink => !string.IsNullOrWhiteSpace(MapUrl)
        ? MapUrl
        : $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(Address)}";
    // دومين الموقع الحقيقي بعد النشر، مثال: https://اسم-الدومين.com
    public string? SiteUrl { get; set; }

    // بنقبل رابط http/https صحيح بس، وبنشيل أي مسار منه
    public string? SiteBase =>
        Uri.TryCreate(SiteUrl, UriKind.Absolute, out var u) &&
        (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps)
            ? u.GetLeftPart(UriPartial.Authority)
            : null;

    // الرقم بالصيغة الدولية: 01040422294 → +201040422294
    public string PhoneInternational
    {
        get
        {
            var digits = new string(Phone.Where(char.IsDigit).ToArray());
            return digits.StartsWith('0') ? "+20" + digits[1..] : "+" + digits;
        }
    }
}