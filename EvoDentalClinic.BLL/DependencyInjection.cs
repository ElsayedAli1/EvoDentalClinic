using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.Mapping;
using EvoDentalClinic.BLL.Services.Implementations;
using EvoDentalClinic.BLL.Services.Implemintation;
using EvoDentalClinic.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<IPatientService, PatientService>();
            services.AddSingleton<IClinicClock, ClinicClock>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ITreatmentService, TreatmentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPatientAccountService, PatientAccountService>();
            services.AddScoped<IClinicSettingsService, ClinicSettingsService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReviewService, ReviewService>();
            return services;
        }
    }
}
