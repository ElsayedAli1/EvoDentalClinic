using System.Text;
using EvoDentalClinic.PL.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EvoDentalClinic.PL.Controllers;

[AllowAnonymous]
public class SeoController : Controller
{
    private readonly ClinicInfoOptions _clinic;

    public SeoController(IOptions<ClinicInfoOptions> clinic)
    {
        _clinic = clinic.Value;
    }

    [HttpGet("/robots.txt")]
    public IActionResult Robots()
    {
        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");

        // مناطق الموظفين والمرضى: محتاجة تسجيل دخول ومش للفهرسة
        foreach (var path in new[]
        {
            "/Patients", "/Appointments", "/MedicalHistory", "/Treatments", "/Payments",
            "/PatientAccounts", "/Staff", "/ClinicSettings", "/Reports", "/ReviewModeration",
            "/Portal", "/Profile"
        })
        {
            sb.AppendLine($"Disallow: {path}");
        }

        if (_clinic.SiteBase is not null)
        {
            sb.AppendLine();
            sb.AppendLine($"Sitemap: {_clinic.SiteBase}/sitemap.xml");
        }

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }

    [HttpGet("/sitemap.xml")]
    public IActionResult Sitemap()
    {
        var baseUrl = _clinic.SiteBase;
        if (baseUrl is null)
            return NotFound();

        var xml = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
              <url><loc>{baseUrl}/</loc></url>
            </urlset>
            """;

        return Content(xml, "application/xml", Encoding.UTF8);
    }
}