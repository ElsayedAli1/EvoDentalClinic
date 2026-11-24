using System.Diagnostics;
using System.Text.RegularExpressions;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.Models;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboard;
        private readonly IClinicSettingsService _settings;
        private readonly IWebHostEnvironment _env;
        private readonly IReviewService _reviews;

        public HomeController(
            ILogger<HomeController> logger,
            IDashboardService dashboard,
            IClinicSettingsService settings,
            IWebHostEnvironment env,
            IReviewService reviews)
        {
            _logger = logger;
            _dashboard = dashboard;
            _settings = settings;
            _env = env;
            _reviews = reviews;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var isDoctor = User.IsInRole(Roles.Doctor);
            var isStaff = isDoctor || User.IsInRole(Roles.Receptionist);

            // الزائر والمريض بيشوفوا الصفحة العامة. الدكتور والاستقبال بيشوفوا لوحة النهاردة
            if (!isStaff)
            {
                var landing = new LandingViewModel
                {
                    Settings = await _settings.GetAsync(),
                    ClinicImages = GetClinicImages(),
                    DoctorImage = GetDoctorImage(),
                    CaseImages = GetCaseImages(),
                    Reviews = await _reviews.GetPublishedAsync(6)
                };

                return View("Landing", landing);
            }

            var dashboard = await _dashboard.GetAsync(includeFinance: isDoctor);
            return View(dashboard);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult HttpStatus(int code)
        {
            if (code == 404)
                return View("NotFound");

            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // ---------- الصور: كل الصور اللي في الفولدرات بتظهر لوحدها ----------

        private IEnumerable<string> ImagesIn(string subFolder)
        {
            var folder = Path.Combine(_env.WebRootPath, "images", subFolder);
            if (!Directory.Exists(folder))
                return Enumerable.Empty<string>();

            return Directory.EnumerateFiles(folder)
                .Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(Path.GetFileName)
                .OfType<string>();
        }

        // ترتيب رقمي: clinic-2 قبل clinic-10
        private static string NaturalKey(string name)
            => Regex.Replace(name, @"\d+", m => m.Value.PadLeft(8, '0'));

        private List<string> GetClinicImages()
            => ImagesIn("clinic")
                .OrderBy(n => NaturalKey(n), StringComparer.OrdinalIgnoreCase)
                .Take(30)
                .ToList();

        private string? GetDoctorImage()
            => ImagesIn("doctor")
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

        private List<string> GetCaseImages()
            => ImagesIn("cases")
                .OrderBy(n => NaturalKey(n), StringComparer.OrdinalIgnoreCase)
                .Take(60)
                .ToList();
    }
}