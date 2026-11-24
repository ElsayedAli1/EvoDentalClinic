using System.Globalization;
using EvoDentalClinic.BLL;
using EvoDentalClinic.DAL;
using EvoDentalClinic.DAL.Context;
using EvoDentalClinic.DAL.Entities;
using EvoDentalClinic.DAL.Seed;
using EvoDentalClinic.PL.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace EvoDentalClinic.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var builder = WebApplication.CreateBuilder(args);

            // ---------- Logging ----------
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(builder.Environment.ContentRootPath, "logs", "evodental-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

            // ---------- إعدادات النشر ----------
            // الافتراضي true. على السيرفر قبل تفعيل الشهادة بنخليه false من appsettings.Production.json
            var forceHttps = builder.Configuration.GetValue("ForceHttps", true);
            var secureCookies = !builder.Environment.IsDevelopment() && forceHttps;
            var cookiePolicy = secureCookies ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;

            // مفاتيح حماية الكوكيز بتتحفظ في ملفات، عشان إعادة تشغيل الموقع ما تطلّعش المستخدمين
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddDataProtection()
                    .SetApplicationName("EvoDentalClinic")
                    .PersistKeysToFileSystem(new DirectoryInfo(
                        Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));
            }

            // ---------- MVC + قفل كل الصفحات إلا اللي فيها [AllowAnonymous] ----------
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddAntiforgery(options => options.Cookie.SecurePolicy = cookiePolicy);

            // ---------- الطبقات ----------
            builder.Services.AddDataAccess(
                builder.Configuration.GetConnectionString("DefaultConnection")!);
            builder.Services.AddBusinessLogic();
            builder.Services.Configure<ClinicInfoOptions>(builder.Configuration.GetSection("Clinic"));

            // ---------- Identity ----------
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.RequireUniqueEmail = false;

                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = cookiePolicy;
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(1);
            });

            var app = builder.Build();

            // ---------- إنشاء الجداول (أول تشغيل على السيرفر) ----------
            if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
            {
                using var migrationScope = app.Services.CreateScope();
                var db = migrationScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
            }

            // ---------- حساب الدكتور والأدوار ----------
            using (var scope = app.Services.CreateScope())
            {
                var config = app.Configuration;
                var email = config["SeedDoctor:Email"];
                var password = config["SeedDoctor:Password"];
                var fullName = config["SeedDoctor:FullName"] ?? "Doctor";

                if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
                {
                    await IdentitySeeder.SeedAsync(scope.ServiceProvider, email, password, fullName);
                }
            }

            // ---------- Pipeline ----------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                if (forceHttps)
                    app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");

            if (forceHttps)
                app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();
        }
    }
}